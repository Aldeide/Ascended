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

            AbilitySystemComponent closestFallback = null;
            float closestFallbackDist = float.MaxValue;

            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agent.Transform.gameObject) continue;

                float sqrDist = (agent.Transform.position - comp.transform.position).sqrMagnitude;

                if (comp.CompareTag("Player"))
                {
                    if (sqrDist < minDist)
                    {
                        minDist = sqrDist;
                        closestPlayer = comp.gameObject;
                    }
                }
                else
                {
                    if (sqrDist < closestFallbackDist)
                    {
                        closestFallbackDist = sqrDist;
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
