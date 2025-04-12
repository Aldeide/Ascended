namespace Systems.AbilitySystem.AbilityTasks
{
    public abstract class DurationalAbilityTask : AbstractAbilityTask
    {
        public abstract void Start(int startFrame);
        public abstract void Tick(int frame, int startFrame, int endFrame);
        public abstract void End(int endFrame);
    }
}