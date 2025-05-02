using AbilitySystem.Runtime.Abilities;

namespace AbilitySystem.Runtime.AbilityTasks
{
    public abstract class DurationalAbilityTask : AbilityTask
    {
        public float StartTime;
        public float Duration;
        public DurationalAbilityTask(Ability owningAbility) : base(owningAbility)
        {
        }

        public abstract void Start();
        public abstract void Tick();
        public abstract void End();

    }
}