using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.AbilityTasks
{
    public abstract class AbilityTask
    {
        public Ability OwningAbility { get; private set; }
        public IAbilitySystem OwnerSystem { get; private set; }
        
        public bool IsActive { get; private set; }

        protected void Initialize(Ability owningAbility)
        {
            OwningAbility = owningAbility;
            OwnerSystem = owningAbility.Owner;
        }

        public void ReadyForActivation()
        {
            if (IsActive) return;
            
            IsActive = true;
            OwningAbility.RegisterTask(this);
            Activate();
        }

        protected virtual void Activate()
        {
            // Override in subclasses
        }

        public virtual void TickTask()
        {
            // Override in subclasses if tick is needed
        }

        public void EndTask()
        {
            if (!IsActive) return;
            
            IsActive = false;
            OnDestroy();
            OwningAbility.UnregisterTask(this);
        }

        protected virtual void OnDestroy()
        {
            // Override in subclasses for cleanup
        }
    }
}