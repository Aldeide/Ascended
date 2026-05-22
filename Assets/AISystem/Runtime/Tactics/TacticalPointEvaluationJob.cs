using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace AISystem.Runtime.Tactics
{
    [BurstCompile]
    public struct TacticalPointEvaluationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<TacticalPointData> Points;
        [ReadOnly] public float3 AgentPosition;
        [ReadOnly] public float3 ThreatPosition;
        [ReadOnly] public float3 ThreatForward;
        
        [ReadOnly] public float WeightCover;
        [ReadOnly] public float WeightFlanking;
        [ReadOnly] public float WeightProximity;
        [ReadOnly] public float WeightOccupancyPenalty;
        [ReadOnly] public bool PreferFlanking;

        public NativeArray<float> Scores;

        public void Execute(int index)
        {
            TacticalPointData point = Points[index];
            float3 pointPos = point.Position;
            float3 pointNormal = point.Normal;

            // 1. Proximity Score (decay with distance to agent)
            float distToAgent = math.distance(AgentPosition, pointPos);
            float proximityScore = 1f / (1f + distToAgent);

            // 2. Cover Score (dot product of cover normal with threat direction)
            float3 toThreat = ThreatPosition - pointPos;
            float distToThreat = math.length(toThreat);
            
            float coverScore = 0f;
            if (distToThreat > 0.001f)
            {
                float3 toThreatNorm = toThreat / distToThreat;
                float dot = math.dot(pointNormal, toThreatNorm);
                // We want the cover normal to point towards the threat (dot close to 1)
                coverScore = math.max(0f, dot);
            }

            // 3. Flanking Score (dot product perpendicular to threat's forward facing)
            float flankingScore = 0f;
            if (distToThreat > 0.001f)
            {
                float3 toCoverNorm = (pointPos - ThreatPosition) / distToThreat;
                float3 threatForwardNorm = math.normalize(ThreatForward);
                // Perpendicular positions to the threat's forward direction score high
                float dot = math.dot(threatForwardNorm, toCoverNorm);
                flankingScore = 1f - math.abs(dot);
            }

            // Calculate base weighted score
            float score = 0f;
            score += proximityScore * WeightProximity;
            score += coverScore * WeightCover;
            score += flankingScore * WeightFlanking;

            // 4. Occupancy Penalty
            if (point.IsOccupied)
            {
                score -= WeightOccupancyPenalty;
            }

            Scores[index] = score;
        }
    }
}
