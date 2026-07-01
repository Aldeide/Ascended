using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using AbilitySystem.Scripts;
using UnityEngine;

namespace AISystem.Runtime.Sensors
{
    public class RangeSensor : LocalWorldSensorBase
    {
        public float MaxRange { get; set; } = 2f;
        public float MinRange { get; set; } = 0f;

        public override void Created() {}

        public override void Update() {}

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            ITarget target = null;
            var action = agent.ActionState.Action;
            var data = agent.ActionState.Data;
            if (action != null && data != null && data.Target != null)
            {
                target = data.Target;
            }

            if (target == null)
            {
                // Fallback: look for the closest player
                GameObject closest = null;
                float minDist = float.MaxValue;
                foreach (var comp in AbilitySystemComponent.ActiveInstances)
                {
                    if (!comp.CompareTag("Player")) continue;
                    if (comp.gameObject == null) continue;
                    float d = Vector3.Distance(agent.Transform.position, comp.transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        closest = comp.gameObject;
                    }
                }
                if (closest != null)
                {
                    float dist = Vector3.Distance(agent.Transform.position, closest.transform.position);
                    return dist >= MinRange && dist <= MaxRange;
                }
                return false;
            }

            float distance = Vector3.Distance(agent.Transform.position, target.Position);
            return distance >= MinRange && distance <= MaxRange;
        }
    }
}
