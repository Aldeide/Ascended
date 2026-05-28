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
                // Fallback: search for any active AbilitySystemComponent that is not self
                var components = AbilitySystemComponent.ActiveComponents;
                AbilitySystemComponent closest = null;
                float closestSqDist = float.MaxValue;
                foreach (var comp in components)
                {
                    if (comp == null || comp.gameObject == null) continue;
                    if (comp.gameObject == agent.Transform.gameObject) continue;
                    float sqDist = (agent.Transform.position - comp.transform.position).sqrMagnitude;
                    if (sqDist < closestSqDist)
                    {
                        closestSqDist = sqDist;
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
            float minSqDist = float.MaxValue;
            foreach (var player in players)
            {
                if (player == null) continue;
                float sqDist = (agent.Transform.position - player.transform.position).sqrMagnitude;
                if (sqDist < minSqDist)
                {
                    minSqDist = sqDist;
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
