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
            // Find closest player GameObject
            AbilitySystemComponent closestPlayer = null;
            float minPlayerDistSq = float.MaxValue;
            AbilitySystemComponent closestAny = null;
            float minAnyDistSq = float.MaxValue;

            // ⚡ Bolt Optimization: Replace FindGameObjectsWithTag with ActiveInstances and use sqrMagnitude
            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agent.Transform.gameObject) continue;

                float distSq = (comp.transform.position - agent.Transform.position).sqrMagnitude;

                if (comp.gameObject.CompareTag("Player"))
                {
                    if (distSq < minPlayerDistSq)
                    {
                        minPlayerDistSq = distSq;
                        closestPlayer = comp;
                    }
                }

                if (distSq < minAnyDistSq)
                {
                    minAnyDistSq = distSq;
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
