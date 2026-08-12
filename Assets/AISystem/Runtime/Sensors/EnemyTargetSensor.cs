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
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players == null || players.Length == 0)
            {
                // ⚡ Bolt: Use O(1) static registry instead of expensive Object.FindObjectsOfType
                var components = AbilitySystemComponent.ActiveInstances;
                AbilitySystemComponent closest = null;
                float closestDistSqr = float.MaxValue;
                foreach (var comp in components)
                {
                    if (comp == null || comp.gameObject == null) continue;
                    if (comp.gameObject == agent.Transform.gameObject) continue;
                    float distSqr = (agent.Transform.position - comp.transform.position).sqrMagnitude;
                    if (distSqr < closestDistSqr)
                    {
                        closestDistSqr = distSqr;
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
            float minDistSqr = float.MaxValue;
            foreach (var player in players)
            {
                if (player == null) continue;
                float distSqr = (agent.Transform.position - player.transform.position).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    minDistSqr = distSqr;
                    closestPlayer = player;
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
