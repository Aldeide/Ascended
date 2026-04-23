using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using GameplayTags.Runtime;
using Sirenix.Utilities;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    /// <summary>
    /// Responsible for managing abilities within an ability system. Handles the storage,
    /// granting, activation, deactivation, and lifecycle of abilities, while integrating
    /// with the associated <see cref="IAbilitySystem"/> owner.
    /// </summary>
    public class AbilityManager
    {
        private IAbilitySystem _owner;
        public Dictionary<string, Ability> Abilities;
        private List<Ability> _abilitySnapshot;
        private PredictionKey _predictionKey;

        private Dictionary<int, Dictionary<string, AbilitySystem.Runtime.Attributes.AttributeValue>>
            _predictionAttributeSnapshots = new();

        public Action OnAbilityGranted;
        public Action<string, PredictionKey, AbilityData> OnServerTryActivateAbilityRequested;
        public Action<string, AbilityData> OnServerTryUnpredictedAbilityRequested;
        public Action<string, AbilityData> OnNotifyClientActivateAbility;
        public Action<string> OnNotifyClientEndAbility;
        public Action<string> OnServerTryEndAbilityRequested;

        public AbilityManager(IAbilitySystem owner)
        {
            _owner = owner;
            Abilities = new Dictionary<string, Ability>();
            _abilitySnapshot = new List<Ability>();
        }

        public void Tick()
        {
            _abilitySnapshot.AddRange(Abilities.Values);
            foreach (var ability in _abilitySnapshot)
            {
                ability.Tick();
            }

            _abilitySnapshot.Clear();
        }

        public void GrantAbility(AbilityDefinition abilityDefinition)
        {
            if (!abilityDefinition) return;
            try
            {
                if (Abilities.ContainsKey(abilityDefinition.UniqueName)) return;
                var ability = abilityDefinition.ToAbility(_owner);
                Abilities.Add(ability.Definition.UniqueName, ability);
            }
            catch (MissingMethodException e)
            {
                Debug.LogError("Failed to add ability: " + abilityDefinition.GetType().FullName + " / " + e.Message);
            }
        }

        public void GrantAbilityServer(AbilityDefinition abilityDefinition)
        {
            if (!abilityDefinition) return;
            if (!_owner.IsServer()) return;
            GrantAbility(abilityDefinition);
            _owner.ReplicationManager.NotifyClientAbilityGranted(abilityDefinition);
        }

        public void RemoveAbility(AbilityDefinition abilityDefinition)
        {
            if (!abilityDefinition) return;
            RemoveAbility(abilityDefinition.UniqueName);
        }

        public void RemoveAbilityServer(AbilityDefinition abilityDefinition)
        {
            if (!abilityDefinition) return;
            if (!_owner.IsServer()) return;
            RemoveAbility(abilityDefinition);
            _owner.ReplicationManager.NotifyClientAbilityRemoved(abilityDefinition);
        }

        public void RemoveAbility(string abilityName)
        {
            if (!Abilities.Remove(abilityName)) return;
        }

        public bool ForceActivateAbility(string abilityName, AbilityData data = new AbilityData())
        {
            Abilities.TryGetValue(abilityName, out Ability ability);
            if (ability == null) return false;
            return ability.TryActivateAbility(data);
        }
        
        public bool TryActivateAbility(string name, AbilityData data = new AbilityData())
        {
            Abilities.TryGetValue(name, out Ability ability);
            if (ability == null) return false;

            var isClientRequest = _owner.IsLocalClient() && !_owner.IsServer();
            var isServerRequest = _owner.IsServer();

            if (!HasAuthorityToActivate(ability, isClientRequest))
            {
                if (isClientRequest)
                {
                    Debug.LogWarning($"Client attempted to activate {name} but lacks authority (Security Policy: {ability.Definition.NetworkSecurityPolicy})");
                }
                return false;
            }

            // Case 1: ClientOnly ability
            if (ability.Definition.IsLocalAbility())
            {
                if (_owner.IsLocalClient())
                {
                    return ability.TryActivateAbility(data);
                }
                else if (_owner.IsServer())
                {
                    // Server tells client to start their local version
                    OnNotifyClientActivateAbility?.Invoke(name, data);
                    return true;
                }
            }

            // Case 2: Server-only behavior (passive or server-driven)
            if (ability.Definition.NetworkPolicy == AbilityNetworkPolicy.Server)
            {
                if (isServerRequest)
                {
                    return ability.TryActivateAbility(data);
                }
                else if (isClientRequest)
                {
                    // Request server to start it
                    OnServerTryUnpredictedAbilityRequested?.Invoke(name, data);
                    return true;
                }
            }

            // Case 3: Predicted behavior
            if (ability.Definition.HasLocalPrediction())
            {
                if (isServerRequest)
                {
                    // If server starts it directly, it just runs. 
                    // Replicated state will inform the client.
                    return ability.TryActivateAbility(data);
                }
                else if (isClientRequest)
                {
                    var key = PredictionKey.CreatePredictionKey();
                    _predictionAttributeSnapshots[key.currentKey] = _owner.AttributeSetManager.Snapshot();

                    var success = ability.TryActivateAbility(key, data);
                    if (success)
                    {
                        OnServerTryActivateAbilityRequested?.Invoke(name, key, data);
                        return true;
                    }
                    _predictionAttributeSnapshots.Remove(key.currentKey);
                    return false;
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
            Debug.Log("Trying to activate ability: " + name + "as server? " + _owner.IsServer());
            if (!_owner.IsServer()) return false;
            Abilities.TryGetValue(name, out Ability ability);
            if (ability == null) return false;
            Debug.Log("Trying to activate ability 2: " + name + "as server? " + _owner.IsServer());
            return ability.TryActivateAbility(key, data);
        }

        public void EndAbility(string abilityName)
        {
            Abilities.TryGetValue(abilityName, out Ability ability);
            if (ability == null) return;

            var isClientRequest = _owner.IsLocalClient() && !_owner.IsServer();
            var isServerRequest = _owner.IsServer();

            if (!HasAuthorityToTerminate(ability, isClientRequest))
            {
                if (isClientRequest)
                {
                    Debug.LogWarning($"Client attempted to terminate {abilityName} but lacks authority (Security Policy: {ability.Definition.NetworkSecurityPolicy})");
                }
                return;
            }

            if (isClientRequest)
            {
                // Predicted abilities are ended locally immediately. 
                // We notify the server regardless to keep state in sync.
                ability.TryEndAbility();
                OnServerTryEndAbilityRequested?.Invoke(abilityName);
            }
            else if (isServerRequest)
            {
                ability.TryEndAbility();
                // Notify clients (especially the owner for predicted abilities)
                OnNotifyClientEndAbility?.Invoke(abilityName);
            }
        }

        public void EndAbility(PredictionKey key)
        {
            Abilities.Where(kv =>
                    kv.Value.PredictionKey.BaseKey == key.currentKey ||
                    kv.Value.PredictionKey.currentKey == key.currentKey)
                .ForEach(a => a.Value.TryEndAbility());
        }

        public void ForceEndAbility(string abilityName)
        {
            Abilities.TryGetValue(abilityName, out var ability);
            ability?.TryEndAbility();
        }
        
        public void CancelAbilitiesWithTags(Tag[] tags)
        {
            foreach (var ability in Abilities.Values.Where(ability =>
                         ability.Definition.AssetTags.Any(tags.Contains)))
            {
                ability.TryCancelAbility();
            }
        }

        public void NotifyServerResponse(PredictionKey key, bool success)
        {
            if (!_owner.IsLocalClient()) return;

            if (success)
            {
                // Prediction confirmed. Cleanup snapshot.
                _predictionAttributeSnapshots.Remove(key.currentKey);
            }
            else
            {
                // Prediction denied. Rollback.
                if (_predictionAttributeSnapshots.TryGetValue(key.currentKey, out var snapshot))
                {
                    _owner.AttributeSetManager.Restore(snapshot);
                    _predictionAttributeSnapshots.Remove(key.currentKey);
                }

                // End any abilities that were started with this key
                EndAbility(key);

                // Retract any effects that were started with this key
                _owner.EffectManager.RetractPredictedEffect(key);
            }
        }

        public string DebugString()
        {
            return Abilities.Keys.Aggregate("Abilities\n",
                (current, ability) => current + (ability + " (" + Abilities[ability].IsActive + ")\n"));
        }
    }
}