using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    /// <summary>
    /// Represents an ability that uses charges as a resource. The charges determine how often the ability can be activated.
    /// Charges can regenerate over time according to a configured regeneration rate.
    /// </summary>
    /// <remarks>
    /// The ChargesAbility class extends the base Ability class and adds mechanics for managing and regenerating charges.
    /// It defines the maximum number of charges and the regeneration rate, which can both be calculated dynamically through meta attributes or defined statically.
    /// </remarks>
    public class ChargesAbility : Ability
    {
        public int CurrentCharges { get; private set; }
        public float LastRegenTime { get; private set; }
        
        private ChargesAbilityDefinition ChargesDef => (ChargesAbilityDefinition)Definition;

        public ChargesAbility(AbilityDefinition definition, IAbilitySystem owner, int level = 1) 
            : base(definition, owner, level)
        {
            CurrentCharges = GetMaxCharges();
            LastRegenTime = owner.GetTime();
        }

        public int GetMaxCharges()
        {
            var metaName = ChargesDef.MaxChargesMetaAttribute;
            if (string.IsNullOrEmpty(metaName)) return ChargesDef.MaxCharges;
            
            var calculated = CalculateMetaAttributeValue(metaName, ChargesDef.MaxCharges);
            return Mathf.FloorToInt(calculated);
        }

        public float GetRegenRate()
        {
            var metaName = ChargesDef.RegenRateMetaAttribute;
            if (string.IsNullOrEmpty(metaName)) return ChargesDef.RegenRate;
            
            return CalculateMetaAttributeValue(metaName, ChargesDef.RegenRate);
        }

        public override AbilityActivationResult CanActivate()
        {
            var baseResult = base.CanActivate();
            if (baseResult != AbilityActivationResult.Success) return baseResult;

            if (CurrentCharges <= 0) return AbilityActivationResult.NoCharges;
            
            return AbilityActivationResult.Success;
        }

        protected override void ActivateAbility(AbilityData data)
        {
            CurrentCharges--;
            // If we were at max charges, start the regen timer
            if (CurrentCharges == GetMaxCharges() - 1)
            {
                LastRegenTime = Owner.GetTime();
            }
        }

        public override void Tick()
        {
            base.Tick();

            var maxCharges = GetMaxCharges();
            if (CurrentCharges >= maxCharges)
            {
                LastRegenTime = Owner.GetTime();
                return;
            }

            var regenRate = GetRegenRate();
            if (regenRate <= 0) return;

            var timeSinceLastRegen = Owner.GetTime() - LastRegenTime;
            var regenInterval = 1f / regenRate;

            if (timeSinceLastRegen >= regenInterval)
            {
                var chargesToRegen = Mathf.FloorToInt(timeSinceLastRegen / regenInterval);
                CurrentCharges = Mathf.Min(maxCharges, CurrentCharges + chargesToRegen);
                LastRegenTime += chargesToRegen * regenInterval;
            }
        }

        private float CalculateMetaAttributeValue(string metaAttributeName, float baseValue)
        {
            var additive = 0f;
            var multiplicative = 1f;
            var overrideValue = 0f;
            var hasOverride = false;

            var activeEffects = Owner.EffectManager.GetActiveEffects();
            foreach (var effect in activeEffects)
            {
                // If definition specifies required tags, check them
                if (ChargesDef.ModifierRequiredTags != null && ChargesDef.ModifierRequiredTags.Length > 0)
                {
                    if (!Owner.TagManager.HasAllTags(ChargesDef.ModifierRequiredTags)) continue;
                }

                if (effect.Definition.Modifiers == null) continue;
                foreach (var mod in effect.Definition.Modifiers)
                {
                    if (mod.AttributeName == metaAttributeName)
                    {
                        for (int i = 0; i < effect.NumStacks; i++)
                        {
                            var val = mod.Calculate(effect);
                            switch (mod.Operation)
                            {
                                case EffectOperation.Additive: additive += val; break;
                                case EffectOperation.Subtractive: additive -= val; break;
                                case EffectOperation.Multiplicative: multiplicative *= val; break;
                                case EffectOperation.Divisive: if (val != 0) multiplicative /= val; break;
                                case EffectOperation.Override: overrideValue = val; hasOverride = true; break;
                            }
                        }
                    }
                }
            }

            if (hasOverride) return overrideValue;
            return (baseValue + additive) * multiplicative;
        }

        public override void EndAbility()
        {
        }
    }
}
