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
            AbilitySystemComponent closestFallback = null;
            float minDist = float.MaxValue;
            float closestFallbackDist = float.MaxValue;
            bool hasPlayers = false;

            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp == null || comp.gameObject == null || comp.gameObject == agent.Transform.gameObject) continue;

                float dist = Vector3.Distance(agent.Transform.position, comp.transform.position);

                if (comp.gameObject.CompareTag("Player"))
                {
                    hasPlayers = true;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestPlayer = comp;
                    }
                }
                else
                {
                    if (dist < closestFallbackDist)
                    {
                        closestFallbackDist = dist;
                        closestFallback = comp;
                    }
                }
            }

            if (hasPlayers && closestPlayer != null)
            {
                return new TransformTarget(closestPlayer.transform);
            }
            else if (closestFallback != null)
            {
                return new TransformTarget(closestFallback.transform);
            }

            return null;
        }
    }
}
