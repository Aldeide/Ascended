using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using Sirenix.Utilities;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    public class AbilityManager
    {
        private IAbilitySystem _owner;
        public Dictionary<string, Ability> Abilities;
        private List<Ability> _abilitySnapshot;
        private PredictionKey _predictionKey;

        public Action OnAbilityGranted;

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
                if (Abilities.ContainsKey(abilityDefinition.uniqueName)) return;
                var ability = abilityDefinition.ToAbility(_owner);
                Abilities.Add(ability.Definition.uniqueName, ability);
            }
            catch (MissingMethodException e)
            {
                Debug.LogError("Failed to add ability: " + abilityDefinition.GetType().FullName + " / " + e.Message);
            }
        }

        public bool TryActivateAbility(string name, AbilityData data = new AbilityData())
        {
            Abilities.TryGetValue(name, out Ability ability);
            if (ability == null) return false;

            if ((_owner.IsServer() || _owner.IsHost()) && !ability.Definition.IsLocalAbility())
            {
                return ability.TryActivateAbility(data);
            }

            if (ability.Definition.IsLocalAbility() && _owner.IsLocalClient())
            {
                return ability.TryActivateAbility(data);
            }

            if (ability.Definition.HasLocalPrediction() && _owner.IsLocalClient())
            {
                var key = PredictionKey.CreatePredictionKey();
                var success = ability.TryActivateAbility(key, data);
                if (success)
                {
                    _owner.Component.ServerTryActivateAbilityRpc(name, key, data);
                    return true;
                }

                return false;
            }

            return false;
        }

        public bool ServerTryActivateAbilityWithKey(string name, PredictionKey key, AbilityData data)
        {
            if (!_owner.IsServer()) return false;

            Abilities.TryGetValue(name, out Ability ability);
            if (ability == null) return false;
            return ability.TryActivateAbility(key, data);
        }

        public void EndAbility(string abilityName)
        {
            Abilities.TryGetValue(abilityName, out Ability ability);
            ability?.TryEndAbility();
            if (_owner.IsLocalClient() && !_owner.IsHost())
            {
                _owner.Component.ServerTryEndAbilityRpc(abilityName);
            }
        }

        public void EndAbility(PredictionKey key)
        {
            Abilities.Where(kv =>
                    kv.Value.PredictionKey.BaseKey == key.currentKey ||
                    kv.Value.PredictionKey.currentKey == key.currentKey)
                .ForEach(a => a.Value.EndAbility());
        }

        public void CancelAbilitiesWithTags(GameplayTag[] tags)
        {
            foreach (var ability in Abilities.Values.Where(ability =>
                         ability.Definition.AssetTags.Any(tags.Contains)))
            {
                ability.TryCancelAbility();
            }
        }

        public string DebugString()
        {
            return Abilities.Keys.Aggregate("Abilities\n",
                (current, ability) => current + (ability + " (" + Abilities[ability].IsActive + ")\n"));
        }
    }
}