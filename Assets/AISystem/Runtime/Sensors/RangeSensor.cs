using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using AbilitySystem.Scripts;

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
                var components = AbilitySystemComponent.ActiveInstances;
                GameObject closest = null;
                float minDist = float.MaxValue;
                foreach (var comp in components)
                {
                    if (comp == null || comp.gameObject == null) continue;
                    if (!comp.CompareTag("Player")) continue;

                    float sqrDist = (agent.Transform.position - comp.transform.position).sqrMagnitude;
                    if (sqrDist < minDist)
                    {
                        minDist = sqrDist;
                        closest = comp.gameObject;
                    }
                }

                if (closest != null)
                {
                    float dist = (agent.Transform.position - closest.transform.position).magnitude;
                    return dist >= MinRange && dist <= MaxRange;
                }

                return false;
            }


            float distance = Vector3.Distance(agent.Transform.position, target.Position);
            return distance >= MinRange && distance <= MaxRange;
        }
    }
}
