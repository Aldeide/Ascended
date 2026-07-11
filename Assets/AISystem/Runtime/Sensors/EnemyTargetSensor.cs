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
            // Find closest player GameObject using centralized registry
            AbilitySystemComponent closestPlayerComp = null;
            float minPlayerDist = float.MaxValue;
            bool playerFound = false;

            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (!comp.gameObject.CompareTag("Player")) continue;

                playerFound = true;
                float dist = Vector3.Distance(agent.Transform.position, comp.transform.position);
                if (dist < minPlayerDist)
                {
                    minPlayerDist = dist;
                    closestPlayerComp = comp;
                }
            }

            if (!playerFound)
            {
                // Fallback: search for any AbilitySystemComponent that is not self
                var components = AbilitySystemComponent.ActiveInstances;
                AbilitySystemComponent closest = null;
                float closestDist = float.MaxValue;
                foreach (var comp in components)
                {
                    if (comp == null || comp.gameObject == null) continue;
                    if (comp.gameObject == agent.Transform.gameObject) continue;
                    float dist = Vector3.Distance(agent.Transform.position, comp.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = comp;
                    }
                }
                if (closest != null)
                {
                    return new TransformTarget(closest.transform);
                }
                return null;
            }

            if (closestPlayerComp != null)
            {
                return new TransformTarget(closestPlayerComp.transform);
            }

            return null;
        }
    }
}
