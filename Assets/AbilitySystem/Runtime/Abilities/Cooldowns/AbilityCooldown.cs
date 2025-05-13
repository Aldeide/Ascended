using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AbilitySystem.Runtime.Abilities.Cooldowns
{
    /// <summary>
    /// Represents the base class for handling the cooldown mechanics of an ability.
    /// </summary>
    /// <remarks>
    /// This abstract class provides functionality to calculate, activate, and manage ability cooldowns.
    /// Derived classes can override the calculation logic to define specific cooldown durations based on custom logic.
    /// </remarks>
    [Serializable]
    public abstract class AbilityCooldown
    {
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag CooldownTag;
        public EffectDefinition CooldownEffect;
        
        public abstract float Calculate(IAbilitySystem owner);

        public virtual bool CanActivate(IAbilitySystem owner)
        {
            return !owner.TagManager.HasAnyTags(CooldownEffect.grantedTags);
        }

        public virtual bool Activate(IAbilitySystem owner)
        {
            if (!CanActivate(owner)) return false;
            
            var effect = CooldownEffect.ToEffect(owner, owner);
            effect.Activate();
            owner.EffectManager.AddEffect(effect);
            return true;
        }
    }
}