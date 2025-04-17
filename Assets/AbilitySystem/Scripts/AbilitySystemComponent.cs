using System;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Utilities;
using UnityEditor.Presets;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    public class AbilitySystemComponent : MonoBehaviour
    {
        public AbilitySystemDefinition definition;
        
        public IAbilitySystem AbilitySystem { get; private set; }

        public void Initialise()
        {
            AbilitySystem = new AbilitySystemManager();
            foreach (var attributeSet in definition.attributeSets)
            {
                Type type = ReflectionUtil.GetAttributeSetType(attributeSet);
                dynamic set = Activator.CreateInstance(type, AbilitySystem);
                AbilitySystem.AttributeSetManager.AddAttributeSet(set as AttributeSet);
            }

            foreach (var ability in definition.baseAbilities)
            {
                AbilitySystem.AbilityManager.GrantAbility(ability);
            }
        }

        public void Update()
        {
            AbilitySystem.Tick();
        }

        public void TryActivateAbility(string abilityName)
        {
            AbilitySystem.AbilityManager.TryActivateAbility(abilityName);
        }

        public void EndAbility(string abilityName)
        {
            AbilitySystem.AbilityManager.EndAbility(abilityName);
        }
    }
}