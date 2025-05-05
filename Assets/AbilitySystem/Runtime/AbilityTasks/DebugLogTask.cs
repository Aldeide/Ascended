using UnityEngine;

namespace AbilitySystem.Runtime.AbilityTasks
{
    public class DebugLogTask : InstantAbilityTask
    {
        public string Message;
        public override void Execute()
        {
            Debug.Log(Message);
        }
    }
}