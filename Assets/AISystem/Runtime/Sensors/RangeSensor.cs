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
                // OPTIMIZATION: Avoid expensive GameObject.FindGameObjectsWithTag allocations
                // in the hot path by using the AbilitySystemComponent.ActiveInstances registry.
                // Using sqrMagnitude instead of Vector3.Distance avoids redundant Mathf.Sqrt calculations
                // during the search loop.
                AbilitySystemComponent closestPlayer = null;
                float minPlayerSqrDist = float.MaxValue;

                foreach (var comp in AbilitySystemComponent.ActiveInstances)
                {
                    if (comp == null || comp.gameObject == null) continue;

                    if (comp.gameObject.CompareTag("Player"))
                    {
                        float sqrDist = (agent.Transform.position - comp.transform.position).sqrMagnitude;
                        if (sqrDist < minPlayerSqrDist)
                        {
                            minPlayerSqrDist = sqrDist;
                            closestPlayer = comp;
                        }
                    }
                }

                if (closestPlayer != null)
                {
                    float dist = Mathf.Sqrt(minPlayerSqrDist);
                    return dist >= MinRange && dist <= MaxRange;
                }

                return false;
            }

            float distance = Vector3.Distance(agent.Transform.position, target.Position);
            return distance >= MinRange && distance <= MaxRange;
        }
    }
}
