using AbilitySystem.Scripts;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace AISystem.Runtime.Sensors
{
    public class TargetDeadSensor : LocalWorldSensorBase
    {
        public override void Created() {}

        public override void Update() {}

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            if (agent.ActionState == null)
            {
                return true;
            }
            // If the agent doesn't have an action or target, we can assume target is dead or we don't have one
            var action = agent.ActionState.Action;
            var data = agent.ActionState.Data;
            if (action == null || data == null || data.Target == null)
            {
                return true;
            }

            var target = data.Target;
            
            // Check if it's a transform target
            if (target is TransformTarget transformTarget)
            {
                if (transformTarget.Transform == null) return true;
                
                var asc = transformTarget.Transform.GetComponent<AbilitySystemComponent>();
                if (asc != null && asc.IsInitialized)
                {
                    var health = asc.AbilitySystem.AttributeSetManager.GetAttribute("Health");
                    if (health != null && health.CurrentValue <= 0f)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
