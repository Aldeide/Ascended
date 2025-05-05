using System;
using AbilitySystem.Runtime.Abilities;

namespace AbilitySystem.Runtime.AbilityTasks
{
    [Serializable]
    public abstract class DurationalAbilityTask : AbilityTask
    {
        public float StartTime;
        public float Duration;
        
        public abstract void Start();
        public abstract void Tick();
        public abstract void End();

        public void Reset()
        {
            StartTime = -1;
        }

        public bool HasEnded()
        {
            return StartTime + Duration <= _owner.GetTime();
        }
        
        public bool IsActive => StartTime > 0;
    }
}