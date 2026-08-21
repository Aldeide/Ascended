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
            float minPlayerDistSqr = float.MaxValue;
            AbilitySystemComponent closestComp = null;
            float minCompDistSqr = float.MaxValue;

            foreach (var comp in AbilitySystemComponent.ActiveInstances)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agent.Transform.gameObject) continue;

                float distSqr = (agent.Transform.position - comp.transform.position).sqrMagnitude;

                if (comp.gameObject.CompareTag("Player"))
                {
                    if (distSqr < minPlayerDistSqr)
                    {
                        minPlayerDistSqr = distSqr;
                        closestPlayer = comp.gameObject;
                    }
                }
                else
                {
                    if (distSqr < minCompDistSqr)
                    {
                        minCompDistSqr = distSqr;
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
