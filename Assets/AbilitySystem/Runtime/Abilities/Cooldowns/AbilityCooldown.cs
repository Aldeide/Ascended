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

            // TODO: have the effect be predefined for easier replication. (or define a version of the Effect class that
            // can be nicely replicated).
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