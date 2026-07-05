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
            bool hasPlayers = false;
            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp != null && comp.gameObject != null && comp.gameObject.CompareTag("Player"))
                {
                    hasPlayers = true;
                    break;
                }
            }
            if (!hasPlayers)
            {
                // Fallback: search for any AbilitySystemComponent that is not self
                AbilitySystemComponent closest = null;
                float closestDist = float.MaxValue;
                foreach (var comp in AbilitySystemComponent.ActiveInstances)
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

            GameObject closestPlayer = null;
            float minDist = float.MaxValue;
            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (!comp.gameObject.CompareTag("Player")) continue;

                float dist = Vector3.Distance(agent.Transform.position, comp.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestPlayer = comp.gameObject;
                }
            }

            if (closestPlayer != null)
            {
                return new TransformTarget(closestPlayer.transform);
            }

            return null;
        }
    }
}
