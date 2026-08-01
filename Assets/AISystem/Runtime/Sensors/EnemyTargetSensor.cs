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
            // OPTIMIZATION: Avoid expensive GameObject.FindGameObjectsWithTag and Object.FindObjectsOfType
            // allocations in the hot path by using the AbilitySystemComponent.ActiveInstances registry.
            // Using sqrMagnitude instead of Vector3.Distance avoids redundant Mathf.Sqrt calculations.
            AbilitySystemComponent closestPlayer = null;
            float minPlayerSqrDist = float.MaxValue;

            AbilitySystemComponent closestFallback = null;
            float minFallbackSqrDist = float.MaxValue;

            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agent.Transform.gameObject) continue;

                float sqrDist = (agent.Transform.position - comp.transform.position).sqrMagnitude;

                if (comp.gameObject.CompareTag("Player"))
                {
                    if (sqrDist < minPlayerSqrDist)
                    {
                        minPlayerSqrDist = sqrDist;
                        closestPlayer = comp;
                    }
                }
                else
                {
                    if (sqrDist < minFallbackSqrDist)
                    {
                        minFallbackSqrDist = sqrDist;
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
