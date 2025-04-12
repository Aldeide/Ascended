using System;
using FishNet.Object;
using Systems.Abilities;
using Systems.AbilitySystem.Components;

namespace Systems.AbilitySystem.Abilities
{
    public abstract class AbilitySpec
    {
        protected object[] AbilityArguments = Array.Empty<object>();
        
        public AbstractAbility Ability { get; }
        public AbilitySystemComponent Owner { get; protected set; }
        
        public int Level { get; protected set; }
        public bool IsActive { get; private set; }
        public int ActiveCount { get; private set; }
        protected event Action<AbilityActivationResult> _onActivateResult;
        protected event Action _onEndAbility;
        protected event Action _onCancelAbility;
        
        public AbilitySpec(AbstractAbility ability, AbilitySystemComponent owner)
        {
            Ability = ability;
            Owner = owner;
        }
        
        public void Tick()
        {
            if (IsActive)
            {
                AbilityTick();
            }
        }

        protected virtual void AbilityTick()
        {
        }

        public abstract void ActivateAbility(params object[] args);

        public abstract void CancelAbility();

        public abstract void EndAbility();

        public virtual AbilityActivationResult CanActivate()
        {
            // TODO
            return AbilityActivationResult.Success;
        }
        
        public virtual bool TryActivateAbility(params object[] args)
        {
            AbilityArguments = args;
            var result = CanActivate();
            var success = result == AbilityActivationResult.Success;
            if (success)
            {
                IsActive = true;
                ActiveCount++;
                //Owner.GameplayTagAggregator.ApplyGameplayAbilityDynamicTag(this);

                ActivateAbility(AbilityArguments);
            }

            _onActivateResult?.Invoke(result);
            return success;
        }
        
        public virtual void TryEndAbility()
        {
            if (!IsActive) return;
            IsActive = false;
            // Owner.GameplayTagAggregator.RestoreGameplayAbilityDynamicTags(this);
            EndAbility();
            _onEndAbility?.Invoke();
        }
        
        public virtual void TryCancelAbility()
        {
            if (!IsActive) return;
            IsActive = false;
            //Owner.GameplayTagAggregator.RestoreGameplayAbilityDynamicTags(this);
            CancelAbility();
            _onCancelAbility?.Invoke();
        }
        
        public virtual void Dispose()
        {
            _onActivateResult = null;
            _onEndAbility = null;
            _onCancelAbility = null;
        }
        
    }
    
    public abstract class AbilitySpec<T> : AbilitySpec where T : AbstractAbility
    {
        public T Data { get; private set; }

        protected AbilitySpec(T ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            Data = ability;
            Level = ability.Level;
        }
    }
}