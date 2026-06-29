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
            var components = AbilitySystemComponent.ActiveInstances;

            AbilitySystemComponent closestPlayer = null;
            float minPlayerDist = float.MaxValue;

            AbilitySystemComponent closestAny = null;
            float minAnyDist = float.MaxValue;

            foreach (var comp in components)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agent.Transform.gameObject) continue;

                float dist = Vector3.Distance(agent.Transform.position, comp.transform.position);

                if (comp.CompareTag("Player"))
                {
                    if (dist < minPlayerDist)
                    {
                        minPlayerDist = dist;
                        closestPlayer = comp;
                    }
                }

                if (dist < minAnyDist)
                {
                    minAnyDist = dist;
                    closestAny = comp;
                }
            }

            if (closestPlayer != null)
            {
                return new TransformTarget(closestPlayer.transform);
            }

            if (closestAny != null)
            {
                return new TransformTarget(closestAny.transform);
            }

            return null;
        }
    }
}
