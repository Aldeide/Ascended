using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    public class AbilityManager
    {
        private readonly IAbilitySystem _owner;
        public Dictionary<string, Ability> Abilities { get; private set; }

        private readonly Dictionary<int, Dictionary<string, AttributeValue>> _predictionAttributeSnapshots = new();

        public AbilityManager(IAbilitySystem owner)
        {
            _owner = owner;
            Abilities = new Dictionary<string, Ability>();
            Debug.Log($"[AbilityManager] Created for System {owner.GetHashCode()}");
        }

        public Ability GrantAbility(AbilityDefinition abilityDefinition, int level = 1)
        {
            if (abilityDefinition == null) return null;
            if (Abilities.TryGetValue(abilityDefinition.UniqueName, out var existing))
            {
                Debug.Log($"[AbilityManager] Already has ability {abilityDefinition.UniqueName} on server={_owner.IsServer()}");
                return existing;
            }
            Debug.Log($"[AbilityManager] Granting {abilityDefinition.UniqueName} to server={_owner.IsServer()}");
            var ability = abilityDefinition.ToAbility(_owner);
            ability.SetLevel(level);
            Abilities.Add(abilityDefinition.UniqueName, ability);

            if (_owner.IsServer())
            {
                _owner.ReplicationManager.NotifyClientAbilityGranted(abilityDefinition);
            }

            return ability;
        }

        public void RemoveAbility(string abilityName)
        {
            if (Abilities.TryGetValue(abilityName, out var ability))
            {
                var def = ability.Definition;
                if (ability.IsActive) ability.TryEndAbility();
                Abilities.Remove(abilityName);

                if (_owner.IsServer())
                {
                    _owner.ReplicationManager.NotifyClientAbilityRemoved(def);
                }
            }
        }

        public void RemoveAbility(AbilityDefinition abilityDefinition)
        {
            if (abilityDefinition == null) return;
            RemoveAbility(abilityDefinition.UniqueName);
        }

        public void Tick()
        {
            foreach (var ability in Abilities.Values)
            {
                // if (ability.IsActive) Debug.Log($"Ticking active ability: {ability.Definition.UniqueName}");
                ability.Tick();
            }
        }

        public string DebugString()
        {
            if (Abilities.Count == 0) return "No Abilities";
            return string.Join("\n", Abilities.Select(kv => $"{kv.Key}: {kv.Value.DebugString()}"));
        }

        public void CancelAbilitiesWithTags(Tag[] tags, Ability ignoreAbility = null)
        {
            if (tags == null || tags.Length == 0) return;

            foreach (var ability in Abilities.Values.ToList())
            {
                if (ability == ignoreAbility) continue;
                if (ability.Definition.AssetTags == null) continue;
                if (ability.Definition.AssetTags.Any(at => tags.Any(t => t == at)))
                {
                    ability.TryCancelAbility();
                }
            }
        }

        public void CancelAllAbilities()
        {
            foreach (var ability in Abilities.Values.ToList())
            {
                if (ability.IsActive) ability.TryCancelAbility();
            }
        }

        public bool TryActivateAbility(string name, AbilityData data = default)
        {
            if (!Abilities.TryGetValue(name, out var ability)) return false;

            var isClientRequest = _owner.IsLocalClient() && !_owner.IsServer();
            var isServerRequest = _owner.IsServer();

            if (!HasAuthorityToActivate(ability, isClientRequest)) return false;

            // Case 1: ClientOnly ability
            if (ability.Definition.IsLocalAbility())
            {
                if (_owner.IsLocalClient())
                {
                    return ability.TryActivateAbility(data);
                }
                else if (_owner.IsServer())
                {
                    _owner.ReplicationManager.RequestClientActivateAbility(name, data);
                    return true;
                }
                return false;
            }

            // Case 2: Server-only behavior
            if (ability.Definition.NetworkPolicy == AbilityNetworkPolicy.Server)
            {
                if (isServerRequest)
                {
                    return ability.TryActivateAbility(data);
                }
                else if (isClientRequest)
                {
                    _owner.ReplicationManager.RequestAbilityActivationUnpredicted(name, data);
                    return true;
                }
                return false;
            }

            // Case 3: Predicted behavior
            if (ability.Definition.HasLocalPrediction())
            {
                if (isServerRequest)
                {
                    var success = ability.TryActivateAbility(data);
                    // On Host, we don't need to request client activation as it already happened above
                    if (success && !_owner.IsLocalClient())
                    {
                        _owner.ReplicationManager.RequestClientActivateAbility(name, data);
                    }
                    return success;
                }
                else if (isClientRequest)
                {
                    var key = PredictionKey.CreatePredictionKey();
                    _predictionAttributeSnapshots[key.currentKey] = _owner.AttributeSetManager.Snapshot();

                    if (ability.TryActivateAbility(key, data))
                    {
                        _owner.ReplicationManager.RequestAbilityActivation(name, key, data);
                        return true;
                    }
                    _predictionAttributeSnapshots.Remove(key.currentKey);
                }
            }

            return false;
        }

        public static bool HasAuthorityToActivate(Ability ability, bool isClient)
        {
            if (!isClient) return true;
            var policy = ability.Definition.NetworkSecurityPolicy;
            return policy switch
            {
                AbilityNetworkSecurityPolicy.ClientOrServer
                    or AbilityNetworkSecurityPolicy.ServerOnlyTermination => true,
                _ => false
            };
        }

        public static bool HasAuthorityToTerminate(Ability ability, bool isClient)
        {
            if (!isClient) return true;
            var policy = ability.Definition.NetworkSecurityPolicy;
            return policy switch
            {
                AbilityNetworkSecurityPolicy.ClientOrServer or AbilityNetworkSecurityPolicy.ServerOnlyExecution => true,
                _ => false
            };
        }

        public bool ServerTryActivateAbilityWithKey(string name, PredictionKey key, AbilityData data)
        {
            if (!_owner.IsServer()) return false;
            if (!Abilities.TryGetValue(name, out var ability)) return false;
            return ability.TryActivateAbility(key, data);
        }

        public void EndAbility(string abilityName)
        {
            Abilities.TryGetValue(abilityName, out Ability ability);
            if (ability == null) return;

            var isClientRequest = _owner.IsLocalClient() && !_owner.IsServer();
            var isServerRequest = _owner.IsServer();

            if (!HasAuthorityToTerminate(ability, isClientRequest)) return;

            if (isClientRequest)
            {
                ability.TryEndAbility();
                _owner.ReplicationManager.RequestAbilityTermination(abilityName);
            }
            else if (isServerRequest)
            {
                ability.TryEndAbility();
                if (ability.Definition.NetworkPolicy != AbilityNetworkPolicy.Server)
                {
                    _owner.ReplicationManager.RequestClientEndAbility(abilityName);
                }
            }
        }

        public void EndAbility(PredictionKey key)
        {
            var abilitiesToEnd = Abilities.Where(kv =>
                    kv.Value.PredictionKey.BaseKey == key.currentKey ||
                    kv.Value.PredictionKey.currentKey == key.currentKey)
                .ToList();

            foreach (var kv in abilitiesToEnd)
            {
                kv.Value.TryEndAbility();
            }
        }

        public void ForceEndAbility(string abilityName)
        {
            Abilities.TryGetValue(abilityName, out var ability);
            ability?.TryEndAbility();
        }

        public void ForceActivateAbility(string abilityName, AbilityData data = default)
        {
            if (Abilities.TryGetValue(abilityName, out var ability))
            {
                ability.TryActivateAbility(data, true);
            }
        }

        public void NotifyServerResponse(PredictionKey key, bool success)
        {
            if (success)
            {
                _predictionAttributeSnapshots.Remove(key.currentKey);
            }
            else
            {
                if (_predictionAttributeSnapshots.TryGetValue(key.currentKey, out var snapshot))
                {
                    _owner.AttributeSetManager.Restore(snapshot);
                    _predictionAttributeSnapshots.Remove(key.currentKey);
                }

                var abilitiesToEnd = Abilities.Where(kv => kv.Value.PredictionKey.currentKey == key.currentKey).ToList();
                foreach (var kv in abilitiesToEnd)
                {
                    kv.Value.TryCancelAbility();
                }
            }
        }

        public void UpdateAbilityCharges(string abilityName, int charges)
        {
            if (Abilities.TryGetValue(abilityName, out var ability))
            {
                if (ability is not ChargesAbility)
                {
                    Debug.Log("[AbilityManager] Attempting to update charges for a non-charge ability: " + abilityName);
                    return;
                }

                ((ChargesAbility)ability).SetCharges(charges);
            }
        }

        public void AbilityLocalInputPressed(string abilityName)
        {
            if (Abilities.TryGetValue(abilityName, out var ability) && ability.IsActive)
            {
                ability.NotifyInputPressed();
            }
        }

        public void AbilityLocalInputReleased(string abilityName)
        {
            if (Abilities.TryGetValue(abilityName, out var ability) && ability.IsActive)
            {
                ability.NotifyInputReleased();
            }
        }
    }
}