using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Abilities.AbilityActivation;
using AbilitySystem.Runtime.Abilities.Cooldowns;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Runtime.Networking;
using UnityEngine;

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
        public virtual AbilityData Data => AbilityArguments;
        
        public AbilityDefinition Definition { get; }
        public AbilityCooldown Cooldown { get; set; }
        public virtual IAbilitySystem Owner { get; protected set; }
        public bool IsActive { get; set; }
        public int ActiveCount { get; private set; }
        
        public virtual int Level { get; set; }
        
        public PredictionKey PredictionKey { get; private set; }

        private readonly List<Effect> _activatedEffects;
        private readonly List<AbilityTask> _activeTasks;
        
        protected event Action<AbilityActivationResult> _onActivateResult;
        protected event Action _onEndAbility;
        protected event Action _onCancelAbility;
        
        protected Ability()
        {
            _activatedEffects = new List<Effect>();
        }

        protected Ability(AbilityDefinition ability, IAbilitySystem owner, int level = 1)
        {
            Definition = ability;
            Owner = owner;
            IsActive = false;
            Level = level;
            _activatedEffects = new List<Effect>();
            _activeTasks = new List<AbilityTask>();

            if (Definition.AbilityActivation != null)
            {
                if (Definition.AbilityActivation is OnEventActivation activation)
                {
                    var eventType = activation.ActivationEvent.EventType;
                    owner.EventManager?.Subscribe(eventType, OnActivationEvent);
                }
            }
            
            if (Definition.Cooldown != null)
            {
                // Cooldown is now an instantiation of the cooldown handler if needed, 
                // but since it's an abstract class, we just take the reference.
                // The actual cloning happens in the concrete Cooldown implementations if they have state.
                Cooldown = Definition.Cooldown; 
            }
        }
        
        public virtual void Tick()
        {
            if (!IsActive) return;
            AbilityTick();
        }

        protected virtual void AbilityTick()
        {
            for (int i = _activeTasks.Count - 1; i >= 0; i--)
            {
                _activeTasks[i].TickTask();
            }
        }

        public void RegisterTask(AbilityTask task)
        {
            if (!_activeTasks.Contains(task))
            {
                _activeTasks.Add(task);
            }
        }

        public void UnregisterTask(AbilityTask task)
        {
            _activeTasks.Remove(task);
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
            if (Owner.TagManager.IsAbilityBlocked(Definition.AssetTags)) return AbilityActivationResult.BlockedByAbility;
            if (IsOnCooldown()) return AbilityActivationResult.CooldownFailed;
            return AbilityActivationResult.Success;
        }

        public bool CanAffordCost()
        {
            if (Definition.Cost == null) return true;
            foreach (var modifier in Definition.Cost.Modifiers)
            {
                var attribute = modifier.AttributeName.Split(".")[1];
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

                
                if (Definition.NetworkPolicy == AbilityNetworkPolicy.Server && Owner.IsServer())
                {
                    Owner.TagManager.AddAbilityTagsAndNotify(this);
                }
                else
                {
                    Owner.TagManager.AddAbilityTags(this);
                }
                Owner.TagManager.AddAbilityBlockingTags(this);
                
                
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
            
            var tasksToClean = new List<AbilityTask>(_activeTasks);
            foreach (var task in tasksToClean)
            {
                task.EndTask();
            }
            _activeTasks.Clear();

            foreach (var activatedEffect in _activatedEffects)
            {
                Owner.EffectManager.RemoveEffect(activatedEffect);
            }
            if (Definition.NetworkPolicy == AbilityNetworkPolicy.Server && Owner.IsServer())
            {
                Owner.TagManager.RemoveAbilityTagsAndNotify(this);
            }
            else
            {
                Owner.TagManager.RemoveAbilityTags(this);
            }
            Owner.TagManager.RemoveAbilityBlockingTags(this);
            EndAbility();
            _activatedEffects.Clear();
            _onEndAbility?.Invoke();
        }
        
        public virtual void TryCancelAbility()
        {
            if (!IsActive) return;
            IsActive = false;
            
            var tasksToClean = new List<AbilityTask>(_activeTasks);
            foreach (var task in tasksToClean)
            {
                task.EndTask();
            }
            _activeTasks.Clear();

            Owner.TagManager.RemoveAbilityTags(this);
            Owner.TagManager.RemoveAbilityBlockingTags(this);
            CancelAbility();
            _onCancelAbility?.Invoke();
        }

        public virtual void CommitCostAndCooldown()
        {
            if (Definition.Cost == null) return;
            var costEffect = MakeOutgoingEffect(Definition.Cost);
            ApplyEffectToSelf(costEffect);
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
                var data = new CueData { PredictionKey = PredictionKey };
                Owner.PlayCue(cue.CueTag, data, IsPredicted());
            }
        }

        public void ApplyEffects()
        {
            foreach (var grantedEffect in Definition.GrantedEffects)
            {
                var effect = MakeOutgoingEffect(grantedEffect);
                ApplyEffectToSelf(effect);
                _activatedEffects.Add(effect);
            }
        }

        public Effect MakeOutgoingEffect(EffectDefinition definition)
        {
            var context = Owner.MakeEffectContext();
            var effect = Owner.MakeOutgoingEffect(definition, Level, context);
            if (IsPredicted())
            {
                effect.PredictionKey = PredictionKey;
            }
            return effect;
        }

        public EffectApplicationResult ApplyEffectToSelf(Effect effect)
        {
            if (effect.IsPredicted() && !Owner.IsServer())
            {
                Owner.EffectManager.AddPredictedEffect(effect.PredictionKey, effect);
                return EffectApplicationResult.Success;
            }
            return Owner.ApplyEffectToSelf(effect);
        }

        public bool IsPredicted()
        {
            return PredictionKey.IsValidKey();
        }

        public Targeting.TargetDataHandle GetTargetData()
        {
            return AbilityArguments.TargetData;
        }

        public List<T> GetTargetDataItems<T>() where T : Targeting.ITargetData
        {
            var results = new List<T>();
            foreach (var item in GetTargetData().Data)
            {
                if (item is T typedItem)
                {
                    results.Add(typedItem);
                }
            }
            return results;
        }

        public void AddTags()
        {
            foreach (var tag in Definition.ActivationOwnedTags)
            {
                Owner.TagManager.AddTag(tag);
            }
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