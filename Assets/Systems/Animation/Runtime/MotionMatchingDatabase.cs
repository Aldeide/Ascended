using System;
using Unity.Mathematics;
using UnityEngine;

namespace Ascended.Systems.Animation.Runtime
{
    /// <summary>
    /// Represents a single predicted point in the character's trajectory.
    /// </summary>
    [Serializable]
    public struct TrajectoryPointData
    {
        public float3 Position;
        public float3 Velocity;
        public float FacingAngle;
    }

    /// <summary>
    /// Represents a single baked frame of animation, containing all the data 
    /// required for the cost function during the Motion Matching search.
    /// </summary>
    [Serializable]
    public struct PoseData
    {
        // Reference to the source clip (by index in the database's clip array)
        public int ClipIndex;
        public float Time;
        
        // Root motion
        public float3 RootVelocity;
        public float3 RootAngularVelocity;
        
        // Future Trajectory at 0.33s, 0.66s, 1.0s
        public TrajectoryPointData Trajectory0;
        public TrajectoryPointData Trajectory1;
        public TrajectoryPointData Trajectory2;

        // Key joint velocities (Left Foot, Right Foot, Hips)
        public float3 LeftFootVelocity;
        public float3 RightFootVelocity;
        public float3 HipsVelocity;
        
        // Key joint positions (local space)
        public float3 LeftFootPosition;
        public float3 RightFootPosition;
        public float3 HipsPosition;
    }

    [Serializable]
    public struct MotionMatchingClipEntry
    {
        [Tooltip("The source animation clip.")]
        public AnimationClip Clip;
        
        [Tooltip("Nominal forward speed of this clip in m/s (for in-place animations). Set to 0 to extract from root motion.")]
        public float ForwardSpeed;
    }

    /// <summary>
    /// The offline baked database of all animation poses used by the motion matching system.
    /// </summary>
    [CreateAssetMenu(fileName = "MotionMatchingDatabase", menuName = "Ascended/Animation/Motion Matching Database")]
    public class MotionMatchingDatabase : ScriptableObject
    {
        [Tooltip("The source animation clips that were baked into this database.")]
        public MotionMatchingClipEntry[] SourceClips;
        
        [Tooltip("The baked pose data, flat array for fast Burst job queries.")]
        public PoseData[] Poses;
        
        [Tooltip("Frame rate at which the poses were sampled.")]
        public float SampleRate = 30f;
    }
}
