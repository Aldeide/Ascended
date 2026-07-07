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
                var players = GameObject.FindGameObjectsWithTag("Player");
                if (players != null && players.Length > 0)
                {
                    GameObject closest = null;
                    float minDistSqr = float.MaxValue;
                    foreach (var p in players)
                    {
                        float dSqr = (agent.Transform.position - p.transform.position).sqrMagnitude;
                        if (dSqr < minDistSqr)
                        {
                            minDistSqr = dSqr;
                            closest = p;
                        }
                    }
                    if (closest != null)
                    {
                        float distSqr = (agent.Transform.position - closest.transform.position).sqrMagnitude;
                        return distSqr >= (MinRange * MinRange) && distSqr <= (MaxRange * MaxRange);
                    }
                }
                return false;
            }

            float distanceSqr = (agent.Transform.position - target.Position).sqrMagnitude;
            return distanceSqr >= (MinRange * MinRange) && distanceSqr <= (MaxRange * MaxRange);
        }
    }
}
