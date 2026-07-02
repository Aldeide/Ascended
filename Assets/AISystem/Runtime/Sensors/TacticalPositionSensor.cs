using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using AISystem.Runtime.Tactics;
using AbilitySystem.Scripts;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AISystem.Runtime.Sensors
{
    public class TacticalPositionSensor : LocalTargetSensorBase
    {
        public bool PreferFlanking { get; set; }

        public override void Created() {}

        public override void Update() {}

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            if (TacticalPointManager.Instance == null)
                return null;

            var points = TacticalPointManager.Instance.AllPoints;
            if (points == null || points.Count == 0)
                return null;

            // Find the threat (closest player or fallback)
            var threat = FindThreat(agent.Transform);
            if (threat == null)
                return null;

            // Prepare native arrays
            var pointDataArray = TacticalPointManager.Instance.GetPointData(Allocator.TempJob, agent.Transform.gameObject);
            var scoresArray = new NativeArray<float>(pointDataArray.Length, Allocator.TempJob);

            // Configure and schedule Job
            var job = new TacticalPointEvaluationJob
            {
                Points = pointDataArray,
                AgentPosition = agent.Transform.position,
                ThreatPosition = threat.transform.position,
                ThreatForward = threat.transform.forward,
                WeightCover = PreferFlanking ? 0.2f : 1.5f,
                WeightFlanking = PreferFlanking ? 1.5f : 0.2f,
                WeightProximity = 0.5f,
                WeightOccupancyPenalty = 1000f,
                PreferFlanking = PreferFlanking,
                Scores = scoresArray
            };

            // Run job synchronously
            JobHandle handle = job.Schedule(pointDataArray.Length, 8);
            handle.Complete();

            // Find best scored point
            float bestScore = -9999f;
            int bestIndex = -1;
            for (int i = 0; i < scoresArray.Length; i++)
            {
                if (scoresArray[i] > bestScore)
                {
                    bestScore = scoresArray[i];
                    bestIndex = i;
                }
            }

            TacticalPoint bestPoint = null;
            if (bestIndex != -1)
            {
                bestPoint = points[bestIndex];
            }

            Vector3? targetPos = null;
            // Manage point reservation
            TacticalPointManager.Instance.ReleaseAllPointsForAgent(agent.Transform.gameObject);
            if (bestPoint != null)
            {
                TacticalPointManager.Instance.ReservePoint(bestPoint, agent.Transform.gameObject);
                targetPos = bestPoint.Position;
            }

            // Cleanup native arrays
            if (pointDataArray.IsCreated) pointDataArray.Dispose();
            if (scoresArray.IsCreated) scoresArray.Dispose();

            if (targetPos.HasValue)
            {
                if (existingTarget is PositionTarget positionTarget)
                {
                    return positionTarget.SetPosition(targetPos.Value);
                }
                return new PositionTarget(targetPos.Value);
            }

            return null;
        }

        private GameObject FindThreat(Transform agentTransform)
        {
            bool hasPlayer = false;
            GameObject closest = null;
            float minDist = float.MaxValue;
            foreach (var playerComp in AbilitySystemComponent.ActiveInstances)
            {
                if (!playerComp.gameObject.CompareTag("Player")) continue;
                var p = playerComp.gameObject;
                if (p == null) continue;
                hasPlayer = true;
                float dist = Vector3.Distance(agentTransform.position, p.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = p;
                }
            }
            if (hasPlayer && closest != null)
            {
                return closest;
            }

            // Fallback to any AbilitySystemComponent that is not self
            var components = AbilitySystemComponent.ActiveInstances;
            AbilitySystemComponent closestComp = null;
            float closestDist = float.MaxValue;
            foreach (var comp in components)
            {
                if (comp == null || comp.gameObject == null) continue;
                if (comp.gameObject == agentTransform.gameObject) continue;
                float dist = Vector3.Distance(agentTransform.position, comp.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestComp = comp;
                }
            }

            return closestComp != null ? closestComp.gameObject : null;
        }
    }
}
