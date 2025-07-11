using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    [CreateAssetMenu(fileName = "FireAbility", menuName = "AbilitySystem/Abilities/FireAbility")]
    public class FireAbilityDefinition : AbilityDefinition
    {
        public CueDefinition impactVisualEffect;
        public EffectDefinition damageEffect;
        public LayerMask layerMask;
        
        public override Type AbilityType()
        {
            return typeof(FireAbility);
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new FireAbility(this, owner);
        }
    }
}