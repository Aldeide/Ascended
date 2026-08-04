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
            AbilitySystemComponent closestPlayer = null;
            float minPlayerDistSqr = float.MaxValue;

            AbilitySystemComponent closestAny = null;
            float minAnyDistSqr = float.MaxValue;

            // ⚡ Bolt: Use O(1) registry instead of expensive FindObjectsOfType/FindGameObjectsWithTag
            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agent.Transform.gameObject) continue;

                // ⚡ Bolt: Use sqrMagnitude to avoid expensive Mathf.Sqrt calls
                float distSqr = (agent.Transform.position - comp.transform.position).sqrMagnitude;

                if (comp.gameObject.CompareTag("Player"))
                {
                    if (distSqr < minPlayerDistSqr)
                    {
                        minPlayerDistSqr = distSqr;
                        closestPlayer = comp;
                    }
                }
                else
                {
                    if (distSqr < minAnyDistSqr)
                    {
                        minAnyDistSqr = distSqr;
                        closestAny = comp;
                    }
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
