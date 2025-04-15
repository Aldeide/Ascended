using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Scripts;

namespace AbilitySystem.Runtime.Abilities
{
    public abstract class Ability
    {
        protected object[] AbilityArguments = Array.Empty<object>();
        
        public AbilityDefinition Definition { get; }
        public IAbilitySystem Owner { get; protected set; }
        
        public int Level { get; protected set; }
        public bool IsActive { get; private set; }
        public int ActiveCount { get; private set; }
        
        protected event Action<AbilityActivationResult> _onActivateResult;
        protected event Action _onEndAbility;
        protected event Action _onCancelAbility;
        
        public Ability(AbilityDefinition ability, IAbilitySystem owner)
        {
            Definition = ability;
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
                Owner.TagManager.ApplyAbilityTags(this);

                ActivateAbility(AbilityArguments);
            }

            _onActivateResult?.Invoke(result);
            return success;
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

                ActivateAbility(AbilityArguments);
            }

            _onActivateResult?.Invoke(result);
            return success;
        }
        
        public virtual void TryEndAbility()
        {
            if (!IsActive) return;
            IsActive = false;
            Owner.TagManager.RemoveAbilityTags(this);
            EndAbility();
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
        
        public virtual void Dispose()
        {
            _onActivateResult = null;
            _onEndAbility = null;
            _onCancelAbility = null;
        }
    }
}