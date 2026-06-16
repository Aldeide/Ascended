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
            float minPlayerSqrDist = float.MaxValue;

            AbilitySystemComponent closestAny = null;
            float minAnySqrDist = float.MaxValue;

            var agentPos = agent.Transform.position;

            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agent.Transform.gameObject) continue;

                float sqrDist = (agentPos - comp.transform.position).sqrMagnitude;

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
                    if (sqrDist < minAnySqrDist)
                    {
                        minAnySqrDist = sqrDist;
                        closestAny = comp;
                    }
                }
            }

            if (closestPlayer != null)
            {
                return new TransformTarget(closestPlayer.transform);
            }
            else if (closestAny != null)
            {
                return new TransformTarget(closestAny.transform);
            }

            return null;
        }
    }
}
