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
            AbilitySystemComponent closestFallback = null;
            float minPlayerDist = float.MaxValue;
            float minFallbackDist = float.MaxValue;

            foreach (var comp in components)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agent.Transform.gameObject) continue;

                float dist = (agent.Transform.position - comp.transform.position).sqrMagnitude;

                if (comp.gameObject.CompareTag("Player"))
                {
                    if (dist < minPlayerDist)
                    {
                        minPlayerDist = dist;
                        closestPlayer = comp;
                    }
                }
                else
                {
                    if (dist < minFallbackDist)
                    {
                        minFallbackDist = dist;
                        closestFallback = comp;
                    }
                }
            }

            if (closestPlayer != null)
            {
                return new TransformTarget(closestPlayer.transform);
            }

            if (closestFallback != null)
            {
                return new TransformTarget(closestFallback.transform);
            }

            return null;
        }
    }
}
