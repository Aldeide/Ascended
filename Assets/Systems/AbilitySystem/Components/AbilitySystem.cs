using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using Systems.Abilities;
using Systems.AbilitySystem.Abilities;
using UnityEngine;

namespace Systems.AbilitySystem.Components
{
    public class AbilitySystem
    {
        private readonly AbilitySystemComponent _asc;
        private readonly Dictionary<string, AbilitySpec> _abilities = new Dictionary<string, AbilitySpec>();
        private readonly List<AbilitySpec> _cachedAbilities = new List<AbilitySpec>();
        
        public AbilitySystem(AbilitySystemComponent owner)
        {
            _asc = owner;
        }

        public void Tick()
        {
            _cachedAbilities.AddRange(_abilities.Values);
            foreach (var abilitySpec in _cachedAbilities)
            {
                abilitySpec.Tick();
            }
            _cachedAbilities.Clear();
        }
        
        public void GrantAbility(AbstractAbility ability)
        {
            if (_abilities.ContainsKey(ability.Name)) return;
            var abilitySpec = ability.CreateSpec(_asc);
            _abilities.Add(ability.Name, abilitySpec);
        }
        
        public void RemoveAbility(AbstractAbility ability)
        {
            RemoveAbility(ability.Name);
        }

        public void RemoveAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;

            EndAbility(abilityName);
            _abilities[abilityName].Dispose();
            _abilities.Remove(abilityName);
        }
        
        public bool TryActivateAbility(string abilityName, params object[] args)
        {
            Debug.Log("Trying to activate: " + abilityName);
            if (!_abilities.ContainsKey(abilityName))
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"you are trying to activate an ability that does not exist: " +
                    $"abilityName=\"{abilityName}\", GameObject=\"{_asc.name}\", " +
                    $"Preset={(_asc.Preset != null ? _asc.Preset.name : "null")}");
#endif
                return false;
            }

            if (!_abilities[abilityName].TryActivateAbility(args)) return false;
            
            var tags = _abilities[abilityName].Ability.AbilityTags.CancelAbilitiesWithTags;
            foreach (var kv in _abilities)
            {
                var abilityTag = kv.Value.Ability.AbilityTags;
                if (abilityTag.AssetTag.HasAnyTags(tags))
                {
                    _abilities[kv.Key].TryCancelAbility();
                }
            }

            return true;
        }
        
        public void EndAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;
            _abilities[abilityName].TryEndAbility();
        }

        public void CancelAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;
            _abilities[abilityName].TryCancelAbility();
        }

        public List<AbilitySpec> GetAllAbilities()
        {
            return _abilities.Values.ToList();
        }
    }
}