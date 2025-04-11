using System;
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
            return AbilityActivationResult.Success;
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