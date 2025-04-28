using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Scripts;

namespace AbilitySystem.Runtime.Abilities
{
    public abstract class Ability
    {
        protected object[] AbilityArguments = Array.Empty<object>();
        
        public AbilityDefinition Definition { get; }
        public IAbilitySystem Owner { get; protected set; }
        public bool IsActive { get; private set; }
        public int ActiveCount { get; private set; }
        
        public PredictionKey PredictionKey { get; private set; }

        private readonly List<Effect> _activatedEffects;
        
        protected event Action<AbilityActivationResult> _onActivateResult;
        protected event Action _onEndAbility;
        protected event Action _onCancelAbility;
        
        public Ability(AbilityDefinition ability, IAbilitySystem owner)
        {
            Definition = ability;
            Owner = owner;
            IsActive = false;
            _activatedEffects = new List<Effect>();
        }
        
        public void Tick()
        {
            if (!IsActive) return;
            AbilityTick();
        }

        protected virtual void AbilityTick()
        {
        }

        protected abstract void ActivateAbility(params object[] args);

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
            // TODO: cooldown check.
            return AbilityActivationResult.Success;
        }

        public bool CanAffordCost()
        {
            if (Definition.cost == null) return true;
            foreach (var modifier in Definition.cost.modifiers)
            {
                var attribute = modifier.attributeName.Split(".")[1];
                var cost = modifier.Calculate(Definition.cost.ToEffect(Owner, Owner));
                if (Owner.AttributeSetManager.GetAttribute(attribute).CurrentValue < cost)
                {
                    return false;
                }
            }
            return true;
        }

        public bool OwnerHasRequiredTags()
        {
            return Owner.TagManager.HasAllTags(Definition.ActivationRequiredTags);
        }

        public bool OwnerHasBlockingTag()
        {
            return Owner.TagManager.HasAnyTags(Definition.ActivationBlockedTags);
        }
        
        public virtual bool TryActivateAbility(params object[] args)
        {
            return TryActivateAbility(PredictionKey.CreateInvalidPredictionKey(), args); 
        }
        
        public virtual bool TryActivateAbility(PredictionKey key, params object[] args)
        {
            AbilityArguments = args;
            var result = CanActivate();
            var success = result == AbilityActivationResult.Success;
            if (success)
            {
                IsActive = true;
                ActiveCount++;
                Owner.TagManager.ApplyAbilityTags(this);
                PredictionKey = key;
                ApplyEffects();
                Owner.AbilityManager.CancelAbilitiesWithTags(Definition.CancelAbilityTags);
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

        protected virtual void CommitCostAndCooldown()
        {
            if (Definition.cost == null) return;
            Definition.cost.ToEffect(Owner, Owner).Execute();
        }
        
        public virtual void Dispose()
        {
            _onActivateResult = null;
            _onEndAbility = null;
            _onCancelAbility = null;
        }

        public virtual void PlayActivationCues()
        {
            foreach (var cue in Definition.activationCues)
            {
                Owner.PlayCue(cue);
            }
        }

        public void ApplyEffects()
        {
            foreach (var grantedEffect in Definition.grantedEffects)
            {
                var effect = grantedEffect.ToEffect(Owner, Owner);
                effect.Activate();
                _activatedEffects.Add(effect);
                if (PredictionKey.IsValidKey() && !Owner.IsServer())
                {
                    effect.PredictionKey = PredictionKey;
                    Owner.EffectManager.AddPredictedEffect(PredictionKey, effect);
                    return;
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
    }
}