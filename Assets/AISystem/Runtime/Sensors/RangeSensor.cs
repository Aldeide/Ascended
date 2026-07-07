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
                var players = new System.Collections.Generic.List<GameObject>();
                foreach (var instance in AbilitySystem.Scripts.AbilitySystemComponent.ActiveInstances)
                {
                    if (instance != null && instance.gameObject != null && instance.gameObject.CompareTag("Player"))
                    {
                        players.Add(instance.gameObject);
                    }
                }
                if (players != null && players.Count > 0)
                {
                    GameObject closest = null;
                    float minDist = float.MaxValue;
                    foreach (var p in players)
                    {
                        float d = Vector3.Distance(agent.Transform.position, p.transform.position);
                        if (d < minDist)
                        {
                            minDist = d;
                            closest = p;
                        }
                    }
                    if (closest != null)
                    {
                        float dist = Vector3.Distance(agent.Transform.position, closest.transform.position);
                        return dist >= MinRange && dist <= MaxRange;
                    }
                }
                return false;
            }

            float distance = Vector3.Distance(agent.Transform.position, target.Position);
            return distance >= MinRange && distance <= MaxRange;
        }
    }
}
