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
        
        private Dictionary<int, Dictionary<string, AbilitySystem.Runtime.Attributes.AttributeValue>> _predictionAttributeSnapshots = new();

        public Action OnAbilityGranted;
        public Action<string, PredictionKey, AbilityData> OnServerTryActivateAbilityRequested;
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

        public bool TryActivateAbility(string name, AbilityData data = new AbilityData())
        {
            Abilities.TryGetValue(name, out Ability ability);
            if (ability == null) return false;

            if ((_owner.IsServer() || _owner.IsHost()) && !ability.Definition.IsLocalAbility())
            {
                Debug.Log("TryActivateAbility for serverhost: " + name);
                return ability.TryActivateAbility(data);
            }

            if (ability.Definition.IsLocalAbility() && _owner.IsLocalClient())
            {
                return ability.TryActivateAbility(data);
            }

            if (ability.Definition.HasLocalPrediction() && _owner.IsLocalClient())
            {
                var key = PredictionKey.CreatePredictionKey();
                Debug.Log("Predicting" + ability.Definition.name);
                // Snapshot before prediction
                _predictionAttributeSnapshots[key.currentKey] = _owner.AttributeSetManager.Snapshot();
                
                var success = ability.TryActivateAbility(key, data);
                if (success)
                {
                    OnServerTryActivateAbilityRequested?.Invoke(name, key, data);
                    return true;
                }

                // If prediction failed locally, cleanup snapshot
                _predictionAttributeSnapshots.Remove(key.currentKey);
                return false;
            }

            return false;
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
            ability?.TryEndAbility();
            if (_owner.IsLocalClient() && !_owner.IsHost())
            {
                OnServerTryEndAbilityRequested?.Invoke(abilityName);
            }
        }

        public void EndAbility(PredictionKey key)
        {
            Abilities.Where(kv =>
                    kv.Value.PredictionKey.BaseKey == key.currentKey ||
                    kv.Value.PredictionKey.currentKey == key.currentKey)
                .ForEach(a => a.Value.TryEndAbility());
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