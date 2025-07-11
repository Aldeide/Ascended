using System;
using AbilitySystem.Runtime.Abilities;

namespace AbilitySystem.Runtime.AbilityTasks
{
    [Serializable]
    public abstract class InstantAbilityTask : AbilityTask
    {
        public abstract void Execute();
    }
}