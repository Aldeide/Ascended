using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    public class AbilityManager
    {
        private IAbilitySystem _owner;
        private Dictionary<string, Ability> _abilities;
        
        public AbilityManager(IAbilitySystem owner)
        {
            _owner = owner;
            _abilities = new Dictionary<string, Ability>();
        }

        public void GrantAbility(AbilityAsset asset)
        {
            if (!asset) return;
            try
            {
                Debug.Log(asset.AbilityType().FullName);
                var abilityDefinition = Activator.CreateInstance(asset.AbilityType(), args: asset) as AbilityDefinition;
                if (_abilities.ContainsKey(abilityDefinition.Name)) return;
                var ability = abilityDefinition.CreateSpec(_owner);
                _abilities.Add(abilityDefinition.Name, ability);
            }
            catch (MissingMethodException e)
            {
                Debug.LogError("Failed to add ability: " + asset.GetType().FullName + " / " + e.Message);
            }
        }

        public void TryActivateAbility(string name, params object[] args)
        {
            if (_abilities.TryGetValue(name, out Ability ability))
            {
                /*
                if (_owner.IsLocalClient() && ability.Definition.Asset.networkPolicy == AbilityNetworkPolicy.Server)
                {
                    return;
                }

                if (_owner.IsLocalClient() &&
                    ability.Definition.Asset.networkPolicy == AbilityNetworkPolicy.ClientPredicted)
                {
                    
                }
                */
                ability.TryActivateAbility(args);
            }
        }

        public void EndAbility(string abilityName)
        {
            _abilities.TryGetValue(abilityName, out Ability ability);
            ability?.TryEndAbility();
        }

        public void NotifyAbilityActivationFailed(PredictionKey key)
        {
            
        }

        public string DebugString()
        {
            return _abilities.Keys.Aggregate("Abilities\n", (current, ability) => current + (ability + "\n"));
        }
    }
}