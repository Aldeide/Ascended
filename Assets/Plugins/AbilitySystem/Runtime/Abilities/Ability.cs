using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Abilities.AbilityActivation;
using AbilitySystem.Runtime.Abilities.Cooldowns;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Runtime.Networking;

namespace AbilitySystem.Runtime.Abilities
{
    /// <summary>
    /// Represents a base class for defining abilities in the ability system.
    /// This class provides core functionality for managing ability lifecycle,
    /// activation, cancellation, cooldowns, and interaction with the owning
    /// system or entity.
    /// </summary>
    public abstract class Ability
    {
        protected AbilityData AbilityArguments;
        
        public AbilityDefinition Definition { get; }
        public AbilityCooldown Cooldown { get; set; }
        public IAbilitySystem Owner { get; protected set; }
        public bool IsActive { get; private set; }
        public int ActiveCount { get; private set; }
        
        public int Level { get; set; }
        
        public PredictionKey PredictionKey { get; private set; }

        private readonly List<Effect> _activatedEffects;
        
        protected event Action<AbilityActivationResult> _onActivateResult;
        protected event Action _onEndAbility;
        protected event Action _onCancelAbility;
        
        protected Ability(AbilityDefinition ability, IAbilitySystem owner, int level = 1)
        {
            Definition = ability;
            Owner = owner;
            IsActive = false;
            Level = level;
            // TODO: clone cooldown.
            Cooldown = Definition.Cooldown;
            _activatedEffects = new List<Effect>();

            if (Definition.AbilityActivation != null)
            {
                if (Definition.AbilityActivation is OnEventActivation activation)
                {
                    var eventType = activation.ActivationEvent.EventType;
                    owner.EventManager?.Subscribe(eventType, OnActivationEvent);
                }
            }
        }
        
        public void Tick()
        {
            if (!IsActive) return;
            AbilityTick();
        }

        protected virtual void AbilityTick()
        {
        }

        protected abstract void ActivateAbility(AbilityData data);

        protected virtual void OnActivationEvent(GameplayEvent gameplayEvent)
        {
            TryActivateAbility(new AbilityData());
        }
        
        protected virtual void CancelAbility()
        {
            EndAbility();
        }

        public abstract void EndAbility();

        public virtual AbilityActivationResult CanActivate()
        {
            if (!CanAffordCost()) return AbilityActivationResult.CostFailed;
            if (!OwnerHasRequiredTags()) return AbilityActivationResult.MissingRequiredTag;
            if (OwnerHasBlockingTag()) return AbilityActivationResult.BlockedByTag;
            return IsOnCooldown() ? AbilityActivationResult.CooldownFailed :
                AbilityActivationResult.Success;
        }

        public bool CanAffordCost()
        {
            if (Definition.Cost == null) return true;
            foreach (var modifier in Definition.Cost.Modifiers)
            {
                var attribute = modifier.attributeName.Split(".")[1];
                var cost = modifier.Calculate(Definition.Cost.ToEffect(Owner, Owner));
                if (Owner.AttributeSetManager.GetAttribute(attribute).CurrentValue < cost)
                {
                    return false;
                }
            }
            return true;
        }

        public void SetLevel(int level)
        {
            Level = level;
        }

        private bool IsOnCooldown()
        {
            return (Definition.Cooldown != null && !Definition.Cooldown.CanActivate(Owner));
        }

        public bool OwnerHasRequiredTags()
        {
            return Owner.TagManager.HasAllTags(Definition.ActivationRequiredTags);
        }

        public bool OwnerHasBlockingTag()
        {
            return Owner.TagManager.HasAnyTags(Definition.ActivationBlockedTags);
        }
        
        public virtual bool TryActivateAbility(AbilityData data)
        {
            return TryActivateAbility(PredictionKey.CreateInvalidPredictionKey(), data); 
        }
        
        public virtual bool TryActivateAbility(PredictionKey key, AbilityData data)
        {
            AbilityArguments = data;
            var result = CanActivate();
            var success = result == AbilityActivationResult.Success;
            if (success)
            {
                IsActive = true;
                ActiveCount++;
                Owner.TagManager.AddAbilityTags(this);
                PredictionKey = key;
                ApplyEffects();
                Owner.AbilityManager.CancelAbilitiesWithTags(Definition.CancelAbilityTags);
                Cooldown?.Activate(Owner);
                ActivateAbility(AbilityArguments);
            }

            _onActivateResult?.Invoke(result);
            return success;
        }
        
        public virtual void TryEndAbility()
        {
            if (!IsActive) return;
            IsActive = false;
            foreach (var activatedEffect in _activatedEffects)
            {
                Owner.EffectManager.RemoveEffect(activatedEffect);
            }
            Owner.TagManager.RemoveAbilityTags(this);
            EndAbility();
            _activatedEffects.Clear();
            _onEndAbility?.Invoke();
        }
        
        public virtual void TryCancelAbility()
        {
            if (!IsActive) return;
            IsActive = false;
            Owner.TagManager.RemoveAbilityTags(this);
            CancelAbility();
            _onCancelAbility?.Invoke();
        }

        public virtual void CommitCostAndCooldown()
        {
            if (Definition.Cost == null) return;
            Definition.Cost.ToEffect(Owner, Owner).Execute();
        }
        
        public virtual void Dispose()
        {
            _onActivateResult = null;
            _onEndAbility = null;
            _onCancelAbility = null;
        }

        public virtual void PlayActivationCues()
        {
            foreach (var cue in Definition.ActivationCues)
            {
                Owner.PlayCue(cue);
            }
        }

        public void ApplyEffects()
        {
            foreach (var grantedEffect in Definition.GrantedEffects)
            {
                var effect = grantedEffect.ToEffect(Owner, Owner);
                effect.Activate();
                _activatedEffects.Add(effect);
                if (PredictionKey.IsValidKey() && !Owner.IsServer())
                {
                    effect.PredictionKey = PredictionKey;
                    Owner.EffectManager.AddPredictedEffect(PredictionKey, effect);
                    continue;
                }

                if (PredictionKey.IsValidKey())
                {
                    effect.PredictionKey = PredictionKey;
                }
                Owner.EffectManager.AddEffect(effect);
            }
        }

        public bool IsPredicted()
        {
            return PredictionKey.IsValidKey();
        }

        public void RemoveTags()
        {
            foreach (var tag in Definition.ActivationOwnedTags)
            {
                Owner.TagManager.RemoveTag(tag);
            }
        }

        private bool HasActivationAuthority()
        {
            if (Definition.NetworkSecurityPolicy == AbilityNetworkSecurityPolicy.ClientOrServer) return true;
            if (Owner.IsServer())
            {
                return true;
            }
            return Definition.NetworkSecurityPolicy == AbilityNetworkSecurityPolicy.ServerOnlyTermination &&
                   Owner.IsLocalClient();
        }
    }
}