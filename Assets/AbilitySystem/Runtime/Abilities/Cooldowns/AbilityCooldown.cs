using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AbilitySystem.Runtime.Abilities.Cooldowns
{
    [Serializable]
    public abstract class AbilityCooldown
    {
        public float BaseDuration;
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag CooldownTag;
        
        public abstract float Calculate(IAbilitySystem owner);

        public virtual bool CanActivate(IAbilitySystem owner)
        {
            return owner.EffectManager.GetEffect(CooldownTag) == null;
        }
        public virtual bool Activate(IAbilitySystem owner)
        {
            if (!CanActivate(owner)) return false;

            var effectDefinition = ScriptableObject.CreateInstance<EffectDefinition>();
            effectDefinition.assetTags = new[] { CooldownTag };
            effectDefinition.durationType = EffectDurationType.FixedDuration;
            effectDefinition.durationSeconds = Calculate(owner);
            var effect = effectDefinition.ToEffect(owner, owner);
            effect.Activate();
            owner.EffectManager.AddEffect(effect);
            return true;
        }
    }
}