using System;

namespace AbilitySystem.Runtime.Abilities.Cooldowns
{
    [Serializable]
    public abstract class AbilityCooldown
    {
        public float BaseDuration;

        protected float _currentCooldown;
        
        public abstract float Calculate();

        public abstract float RemainingCooldown();

        public abstract void Tick();
    }
}