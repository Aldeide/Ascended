using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
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
                float minDistSqr = float.MaxValue;
                foreach (var comp in AbilitySystem.Scripts.AbilitySystemRegistry.AllComponents)
                {
                    if (comp != null && comp.gameObject != null && comp.gameObject.CompareTag("Player"))
                    {
                        float dSqr = (agent.Transform.position - comp.transform.position).sqrMagnitude;
                        if (dSqr < minDistSqr)
                        {
                            minDistSqr = dSqr;
                            closest = comp.gameObject;
                        }
                    }
                }

                if (closest != null)
                {
                    float dist = Mathf.Sqrt(minDistSqr);
                    return dist >= MinRange && dist <= MaxRange;
                }

                return false;
            }

            float distanceSqr = (agent.Transform.position - target.Position).sqrMagnitude;
            // Since MinRange and MaxRange could be negative, though unlikely, standard distance is safer for boolean check
            // However, assuming they are >= 0, we can check squares
            float minSqr = MinRange * MinRange;
            float maxSqr = MaxRange * MaxRange;
            return distanceSqr >= minSqr && distanceSqr <= maxSqr;
        }
    }
}
