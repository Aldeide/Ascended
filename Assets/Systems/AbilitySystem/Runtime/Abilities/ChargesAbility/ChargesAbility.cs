using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using UnityEngine;
using AbilitySystem.Runtime.Attributes;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

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
        private bool _wasOnCooldown;
        
        private ChargesAbilityDefinition ChargesDef => (ChargesAbilityDefinition)Definition;

        public ChargesAbility(AbilityDefinition definition, IAbilitySystem owner, int level = 1) 
            : base(definition, owner, level)
        {
            CurrentCharges = GetMaxCharges();
            _wasOnCooldown = IsOnCooldown();
            Debug.Log($"[ChargesAbility] Initialized {definition.UniqueName}. CurrentCharges: {CurrentCharges}");
        }

        public int GetMaxCharges()
        {
            var metaName = ChargesDef.MaxChargesMetaAttribute;
            if (string.IsNullOrEmpty(metaName)) return ChargesDef.MaxCharges;
            
            var calculated = CalculateMetaAttributeValue(metaName, ChargesDef.MaxCharges);
            return Mathf.FloorToInt(calculated);
        }

        public override AbilityActivationResult CanActivate()
        {
            // We bypass the standard IsOnCooldown check because the cooldown timer 
            // is used for charge regeneration, not for blocking activation.
            if (!CanAffordCost()) return AbilityActivationResult.CostFailed;
            if (!OwnerHasRequiredTags()) return AbilityActivationResult.MissingRequiredTag;
            if (OwnerHasBlockingTag()) return AbilityActivationResult.BlockedByTag;
            if (Owner.TagManager.IsAbilityBlocked(Definition.AssetTags)) return AbilityActivationResult.BlockedByAbility;

            if (GetCurrentCharges() <= 0) return AbilityActivationResult.NoCharges;
            
            return AbilityActivationResult.Success;
        }

        protected override void ActivateAbility(AbilityData data)
        {
            CurrentCharges--;
            Debug.Log($"[ChargesAbility] Consuming charge for {Definition.UniqueName}. New count: {CurrentCharges}");
            // Cooldown is automatically activated by the Tick() logic when CurrentCharges < maxCharges
        }

        protected override bool ShouldActivateCooldownOnActivation() => false;

        public int GetCurrentCharges()
        {
            var metaName = ChargesDef.AbilityChargesMetaAttribute;
            if (string.IsNullOrEmpty(metaName)) return CurrentCharges;
            
            var calculated = CalculateMetaAttributeValue(metaName, CurrentCharges);
            return Mathf.Max(0, Mathf.FloorToInt(calculated));
        }

        public override void Tick()
        {
            if (IsActive)
            {
                AbilityTick();
            }

            SyncWithMetaAttribute();

            var maxCharges = GetMaxCharges();
            bool isOnCooldown = IsOnCooldown();
            
            if (Definition.UniqueName == "Dash" || Definition.UniqueName == "TestChargesAbility")
            {
                Debug.Log($"[ChargesAbility] Tick Start {Definition.UniqueName} - Current: {CurrentCharges}, Max: {maxCharges}, OnCooldown: {isOnCooldown}, WasOnCooldown: {_wasOnCooldown}");
            }

            if (CurrentCharges < maxCharges)
            {
                // If a cooldown just finished, we gain a charge
                if (_wasOnCooldown && !isOnCooldown)
                {
                    CurrentCharges++;
                    Debug.Log($"[ChargesAbility] Charge gained for {Definition.UniqueName}. New count: {CurrentCharges}");
                    
                    // If we still need more charges, restart the cooldown immediately
                    if (CurrentCharges < maxCharges)
                    {
                        Cooldown?.Activate(Owner);
                        isOnCooldown = true;
                    }
                }
                // If we are below max but no recharge is happening, start it
                else if (!isOnCooldown)
                {
                    Cooldown?.Activate(Owner);
                    isOnCooldown = true;
                }
            }

            if (Definition.UniqueName == "Dash" || Definition.UniqueName == "TestChargesAbility")
            {
                Debug.Log($"[ChargesAbility] Tick End {Definition.UniqueName} - Current: {CurrentCharges}, OnCooldown: {isOnCooldown}");
            }

            _wasOnCooldown = isOnCooldown;
        }

        private void SyncWithMetaAttribute()
        {
            var metaName = ChargesDef.AbilityChargesMetaAttribute;
            if (string.IsNullOrEmpty(metaName)) return;

            var splits = metaName.Split(".");
            Attribute attr;
            if (splits.Length == 2)
            {
                attr = Owner.AttributeSetManager.GetAttribute(splits[0], splits[1]);
            }
            else
            {
                attr = Owner.AttributeSetManager.GetAttribute(metaName);
            }
            attr?.SetBaseValue(CurrentCharges);
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
        public override string DebugString()
        {
            return base.DebugString() + $" [Charges: {GetCurrentCharges()}/{GetMaxCharges()}]";
        }
    }
}
