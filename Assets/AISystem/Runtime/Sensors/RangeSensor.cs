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
                    float minDist = float.MaxValue;
                    foreach (var p in players)
                    {
                        float d = (agent.Transform.position - p.transform.position).sqrMagnitude;
                        if (d < minDist)
                        {
                            minDist = d;
                            closest = p;
                        }
                    }
                    if (closest != null)
                    {
                        float distSq = (agent.Transform.position - closest.transform.position).sqrMagnitude;
                        return distSq >= (MinRange * MinRange) && distSq <= (MaxRange * MaxRange);
                    }
                }
                return false;
            }

            float distanceSq = (agent.Transform.position - target.Position).sqrMagnitude;
            return distanceSq >= (MinRange * MinRange) && distanceSq <= (MaxRange * MaxRange);
        }
    }
}
