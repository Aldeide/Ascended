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
            AbilitySystemComponent closestComp = null;
            float closestDist = float.MaxValue;
            bool foundPlayer = false;

            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agent.Transform.gameObject) continue;

                if (comp.gameObject.CompareTag("Player"))
                {
                    foundPlayer = true;
                    float dist = (agent.Transform.position - comp.transform.position).sqrMagnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestPlayer = comp.gameObject;
                    }
                }
                else if (!foundPlayer)
                {
                    float dist = (agent.Transform.position - comp.transform.position).sqrMagnitude;
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestComp = comp;
                    }
                }
            }

            if (closestPlayer != null)
            {
                return new TransformTarget(closestPlayer.transform);
            }
            if (closestComp != null)
            {
                return new TransformTarget(closestComp.transform);
            }

            return null;
        }
    }
}
