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
            AbilitySystemComponent closestComp = null;
            float minPlayerDist = float.MaxValue;
            float minCompDist = float.MaxValue;

            // Iterate over the centralized registry to find the closest Player or Fallback target
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
                    if (dist < minCompDist)
                    {
                        minCompDist = dist;
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
