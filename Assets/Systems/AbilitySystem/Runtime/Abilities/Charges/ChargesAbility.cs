using System;
using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using UnityEngine;
using AbilitySystem.Runtime.Attributes;
using GameplayTags.Runtime;
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
        private int _lastEffectiveCharges;
        private int _lastMaxCharges;
        
        public event Action<int, int> OnChargesChanged; // (current, max)
        public event Action<float> OnCooldownStarted; // (duration)
        public event Action<float> OnCooldownProgressChanged; // (0-1 progress)

        private ChargesAbilityDefinition ChargesDef => (ChargesAbilityDefinition)Definition;

        public ChargesAbility(AbilityDefinition definition, IAbilitySystem owner, int level = 1) 
            : base(definition, owner, level)
        {
            CurrentCharges = GetMaxCharges();
            _wasOnCooldown = IsOnCooldown();
            _lastEffectiveCharges = GetCurrentCharges();
            _lastMaxCharges = GetMaxCharges();
            Debug.Log($"[ChargesAbility] Initialized {definition.UniqueName} on server={owner.IsServer()} (Hash:{GetHashCode()}). CurrentCharges: {CurrentCharges}");

            // On clients, fire OnCooldownStarted when the cooldown effect arrives from the server.
            // The server fires it directly in StartCooldown().
            if (!owner.IsServer())
            {
                owner.EffectManager.OnEffectAdded += OnEffectAddedHandler;
            }
        }

        private void OnEffectAddedHandler(Effect effect)
        {
            if (Cooldown == null) return;
            if (effect.Definition != Definition.Cooldown?.CooldownEffect) return;
            OnCooldownStarted?.Invoke(effect.Duration);
        }

        public int GetMaxCharges()
        {
            var metaName = ChargesDef.MaxChargesMetaAttribute;
            if (string.IsNullOrEmpty(metaName)) return ChargesDef.MaxCharges;
            
            var calculated = CalculateMetaAttributeValue(metaName, ChargesDef.MaxCharges, ChargesDef.MaxChargesModifiersTagQuery);
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
            Debug.Log($"[ChargesAbility] Consuming charge for {Definition.UniqueName} on server={Owner.IsServer()} (Hash:{GetHashCode()}). New count: {CurrentCharges}");
            NotifyChargesChanged();
            // Cooldown is automatically activated by the Tick() logic when CurrentCharges < maxCharges
        }

        protected override bool ShouldActivateCooldownOnActivation() => false;

        public int GetCurrentCharges()
        {
            return CurrentCharges;
        }

        public void SetCharges(int current, int max = -1)
        {
            CurrentCharges = current;
            if (max != -1) _lastMaxCharges = max;
            NotifyChargesChanged();
        }

        public override void Tick()
        {
            if (IsActive)
            {
                AbilityTick();
            }

            var maxCharges = GetMaxCharges();
            var isOnCooldown = IsOnCooldown();

            if (CurrentCharges < maxCharges)
            {
                // If a cooldown just finished, we gain a charge
                if (_wasOnCooldown && !isOnCooldown)
                {
                    CurrentCharges++;
                    NotifyChargesChanged();
                    
                    // Only the server restarts the cooldown for the next charge.
                    // Clients rely on server-replicated effects — starting a local cooldown
                    // here would block future regen by keeping IsOnCooldown() true.
                    if (Owner.IsServer() && CurrentCharges < maxCharges)
                    {
                        StartCooldown(maxCharges);
                        isOnCooldown = true;
                    }
                }
                // If we are below max but no recharge is happening, start it (server only)
                else if (!isOnCooldown && Owner.IsServer())
                {
                    StartCooldown(maxCharges);
                    isOnCooldown = true;
                }

                if (isOnCooldown)
                {
                    UpdateCooldownProgress();
                }
            }

            // Check if max charges changed via attributes
            if (maxCharges != _lastMaxCharges)
            {
                _lastMaxCharges = maxCharges;
                NotifyChargesChanged();
            }

            _wasOnCooldown = isOnCooldown;
        }

        private void StartCooldown(int maxCharges)
        {
            if (Cooldown == null) return;
            Cooldown.Activate(Owner);
            OnCooldownStarted?.Invoke(Cooldown.Calculate(Owner));
        }

        private void UpdateCooldownProgress()
        {
            if (Cooldown == null) return;
            var activeCooldownEffect = Owner.EffectManager.GetActiveEffects()
                .Where(e => e.Definition.GrantedTags != null && 
                            Definition.Cooldown.CooldownEffect.GrantedTags != null &&
                            e.Definition.GrantedTags.Intersect(Definition.Cooldown.CooldownEffect.GrantedTags).Any())
                .OrderByDescending(e => e.ActivationTime)
                .FirstOrDefault();

            if (activeCooldownEffect == null) return;
            var duration = activeCooldownEffect.Duration;
            if (!(duration > 0)) return;
            var remaining = activeCooldownEffect.RemainingDuration();
            var progress = 1f - (remaining / duration);
            OnCooldownProgressChanged?.Invoke(progress);
        }

        private void NotifyChargesChanged()
        {
            var current = GetCurrentCharges();
            var max = GetMaxCharges();
            _lastEffectiveCharges = current;
            _lastMaxCharges = max;
            
            if (Owner.IsServer())
            {
                Owner.ReplicationManager.NotifyClientsAbilityChargesChanged(Definition.UniqueName, current, max);
            }
            
            OnChargesChanged?.Invoke(current, max);
        }

        private float CalculateMetaAttributeValue(string metaAttributeName, float baseValue, TagQuery tagQuery)
        {
            var additive = 0f;
            var multiplicative = 1f;
            var overrideValue = 0f;
            var hasOverride = false;

            var activeEffects = Owner.EffectManager.GetActiveEffects();
            foreach (var effect in activeEffects)
            {
                if (!tagQuery.MatchesTags(effect.Definition.AssetTags)) continue;

                if (effect.Definition.Modifiers == null) continue;
                foreach (var mod in effect.Definition.Modifiers)
                {
                    if (mod.AttributeName != metaAttributeName) continue;
                    for (var i = 0; i < effect.NumStacks; i++)
                    {
                        var val = mod.Calculate(effect);
                        switch (mod.Operation)
                        {
                            case EffectOperation.Additive: additive += val; break;
                            case EffectOperation.Subtractive: additive -= val; break;
                            case EffectOperation.Multiplicative: multiplicative *= val; break;
                            case EffectOperation.Divisive: if (val != 0) multiplicative /= val; break;
                            case EffectOperation.Override: overrideValue = val; hasOverride = true; break;
                            default:
                                throw new ArgumentOutOfRangeException();
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
