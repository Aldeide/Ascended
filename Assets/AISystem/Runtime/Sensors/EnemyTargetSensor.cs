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
            float minPlayerDist = float.MaxValue;

            AbilitySystemComponent closestComp = null;
            float closestCompDist = float.MaxValue;

            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agent.Transform.gameObject) continue;

                float dist = Vector3.Distance(agent.Transform.position, comp.transform.position);

                if (comp.gameObject.CompareTag("Player"))
                {
                    if (dist < minPlayerDist)
                    {
                        minPlayerDist = dist;
                        closestPlayer = comp.gameObject;
                    }
                }
                else
                {
                    if (dist < closestCompDist)
                    {
                        closestCompDist = dist;
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
