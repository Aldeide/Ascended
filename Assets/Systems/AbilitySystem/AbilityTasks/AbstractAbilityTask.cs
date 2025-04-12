using Systems.AbilitySystem.Abilities;

namespace Systems.AbilitySystem.AbilityTasks
{
    public abstract class AbstractAbilityTask
    {
        protected AbilitySpec AbilitySpec;

        public virtual void Initialise(AbilitySpec abilitySpec)
        {
            AbilitySpec = abilitySpec;
        }
    }
}