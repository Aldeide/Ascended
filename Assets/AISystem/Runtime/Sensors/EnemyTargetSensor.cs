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
            // Bolt: Optimize FindGameObjectsWithTag and FindObjectsOfType by using the centralized active instance registry.
            // Also optimized distance calculations to use sqrMagnitude to avoid expensive square root operations.
            var components = AbilitySystemComponent.ActiveInstances;

            AbilitySystemComponent closestPlayer = null;
            float minPlayerDist = float.MaxValue;

            AbilitySystemComponent closestAny = null;
            float minAnyDist = float.MaxValue;

            bool hasPlayers = false;

            foreach (var comp in components)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agent.Transform.gameObject) continue;

                float distSq = (agent.Transform.position - comp.transform.position).sqrMagnitude;

                if (comp.gameObject.CompareTag("Player"))
                {
                    hasPlayers = true;
                    if (distSq < minPlayerDist)
                    {
                        minPlayerDist = distSq;
                        closestPlayer = comp;
                    }
                }

                if (distSq < minAnyDist)
                {
                    minAnyDist = distSq;
                    closestAny = comp;
                }
            }

            if (hasPlayers && closestPlayer != null)
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
