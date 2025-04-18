using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using Sirenix.Utilities;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    public class AbilityManager
    {
        private IAbilitySystem _owner;
        private Dictionary<string, Ability> _abilities;
        private PredictionKey _predictionKey;

        public AbilityManager(IAbilitySystem owner)
        {
            _owner = owner;
            _abilities = new Dictionary<string, Ability>();
        }

        public void GrantAbility(AbilityDefinition abilityDefinition)
        {
            if (!abilityDefinition) return;
            try
            {
                if (_abilities.ContainsKey(abilityDefinition.uniqueName)) return;
                var ability = abilityDefinition.ToAbility(_owner);
                _abilities.Add(ability.Definition.uniqueName, ability);
            }
            catch (MissingMethodException e)
            {
                Debug.LogError("Failed to add ability: " + abilityDefinition.GetType().FullName + " / " + e.Message);
            }
        }

        public bool TryActivateAbility(string name, params object[] args)
        {
            _abilities.TryGetValue(name, out Ability ability);
            if (ability == null) return false;

            if (_owner.IsServer() && !ability.Definition.IsLocalAbility())
            {
                return ability.TryActivateAbility(args);
            }
            
            if (ability.Definition.IsLocalAbility() && _owner.IsLocalClient())
            {
                return ability.TryActivateAbility(args);
            }

            if (ability.Definition.HasLocalPrediction() && _owner.IsLocalClient())
            {
                var key = PredictionKey.CreatePredictionKey();
                var success = ability.TryActivateAbility(key, args);
                if (success)
                {
                    _owner.Component.ServerTryActivateAbilityRpc(name, key);
                    return true;
                }

                return false;
            }

            return false;
        }

        public void EndAbility(string abilityName)
        {
            _abilities.TryGetValue(abilityName, out Ability ability);
            ability?.TryEndAbility();
        }

        public void EndAbility(PredictionKey key)
        {
            _abilities.Where(kv =>
                    kv.Value.PredictionKey.BaseKey == key.currentKey ||
                    kv.Value.PredictionKey.currentKey == key.currentKey)
                .ForEach(a => a.Value.EndAbility());
        }

        public string DebugString()
        {
            return _abilities.Keys.Aggregate("Abilities\n", (current, ability) => current + (ability + "\n"));
        }
    }
}