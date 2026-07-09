using AbilitySystem.Scripts;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace AISystem.Runtime.Sensors
{
    public class EnemyTargetSensor : LocalTargetSensorBase
    {
        public override void Created() {}

        public override void Update() {}

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            GameObject closestPlayer = null;
            float minDist = float.MaxValue;
            var components = AbilitySystemComponent.ActiveInstances;

            foreach (var comp in components)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (!comp.CompareTag("Player")) continue;

                float sqrDist = (agent.Transform.position - comp.transform.position).sqrMagnitude;
                if (sqrDist < minDist)
                {
                    minDist = sqrDist;
                    closestPlayer = comp.gameObject;
                }
            }

            if (closestPlayer == null)
            {
                AbilitySystemComponent closest = null;
                float closestDist = float.MaxValue;
                foreach (var comp in components)
                {
                    if (comp == null || comp.gameObject == null) continue;
                    if (comp.gameObject == agent.Transform.gameObject) continue;
                    float sqrDist = (agent.Transform.position - comp.transform.position).sqrMagnitude;
                    if (sqrDist < closestDist)
                    {
                        closestDist = sqrDist;
                        closest = comp;
                    }
                }
                if (closest != null)
                {
                    return new TransformTarget(closest.transform);
                }
                return null;
            }

            if (closestPlayer != null)
            {
                return new TransformTarget(closestPlayer.transform);
            }

            return null;
        }
    }
}
