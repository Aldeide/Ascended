using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

namespace Ascended.Systems.Animation.Runtime
{
    /// <summary>
    /// Runtime Motion Matching evaluation engine.
    /// Periodically runs a Burst-compiled job to find the lowest-cost pose in the database
    /// and uses the PlayableGraph API to crossfade to it.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AscendedTrajectoryGenerator))]
    public class AscendedMotionMatcher : MonoBehaviour
    {
        [Header("Database")]
        [Tooltip("The offline baked pose database.")]
        [SerializeField] private MotionMatchingDatabase _database;

        [Header("Search Settings")]
        [Tooltip("How often to query the database for a better matching pose (in seconds).")]
        [Range(0.033f, 0.5f)]
        [SerializeField] private float _updateInterval = 0.1f;

        [Tooltip("The crossfade duration when switching to a new animation pose.")]
        [Range(0.05f, 0.5f)]
        [SerializeField] private float _blendDuration = 0.2f;

        [Header("Cost Weights")]
        [SerializeField] private float _trajectoryPositionWeight = 1.0f;
        [SerializeField] private float _trajectoryVelocityWeight = 0.5f;
        [SerializeField] private float _trajectoryFacingWeight = 0.5f;
        [SerializeField] private float _posePositionWeight = 0.5f;
        [SerializeField] private float _poseVelocityWeight = 0.2f;

        [Header("Biases & Penalties")]
        [Tooltip("Penalty added to the cost if the candidate pose is from a different clip (prevents rapid twitching).")]
        [SerializeField] private float _clipSwitchPenalty = 0.1f;
        [Tooltip("Penalty added to the cost if the candidate pose is a jump in time within the same clip (favors continuity).")]
        [SerializeField] private float _timeJumpPenalty = 0.05f;

        private Animator _animator;
        private AscendedTrajectoryGenerator _trajectoryGenerator;

        // Native memory for Burst jobs
        private NativeArray<PoseData> _posesNative;
        private NativeArray<int> _bestPoseIndexOut;

        // Playables variables
        private PlayableGraph _playableGraph;
        private AnimationMixerPlayable _mixer;
        private AnimationClipPlayable _currentPlayable;
        private AnimationClipPlayable _nextPlayable;

        // State tracking
        private float _searchTimer;
        private float _blendTimer;
        private int _currentClipIndex = -1;

        // Previous frames positions for joint velocity calculations
        private float3 _prevHipsPos;
        private float3 _prevLeftFootPos;
        private float3 _prevRightFootPos;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _trajectoryGenerator = GetComponent<AscendedTrajectoryGenerator>();
        }

        private void OnEnable()
        {
            InitializeDatabase();
            InitializePlayableGraph();
            
            // Initialize bone tracking positions to prevent massive velocities on first frame
            Transform hips = _animator.GetBoneTransform(HumanBodyBones.Hips);
            Transform leftFoot = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform rightFoot = _animator.GetBoneTransform(HumanBodyBones.RightFoot);
            if (hips != null) _prevHipsPos = transform.InverseTransformPoint(hips.position);
            if (leftFoot != null) _prevLeftFootPos = transform.InverseTransformPoint(leftFoot.position);
            if (rightFoot != null) _prevRightFootPos = transform.InverseTransformPoint(rightFoot.position);

            // Start playing the first clip if available
            if (_database != null && _database.SourceClips != null && _database.SourceClips.Length > 0)
            {
                TransitionToPose(0, 0f);
            }
        }

        private void OnDisable()
        {
            CleanupDatabase();
            CleanupPlayableGraph();
        }

        private void InitializeDatabase()
        {
            if (_database == null || _database.Poses == null || _database.Poses.Length == 0)
            {
                Debug.LogWarning("[AscendedMotionMatcher] Motion Matching Database is unassigned or empty.");
                return;
            }

            _posesNative = new NativeArray<PoseData>(_database.Poses, Allocator.Persistent);
            _bestPoseIndexOut = new NativeArray<int>(1, Allocator.Persistent);
        }

        private void CleanupDatabase()
        {
            if (_posesNative.IsCreated) _posesNative.Dispose();
            if (_bestPoseIndexOut.IsCreated) _bestPoseIndexOut.Dispose();
        }

        private void InitializePlayableGraph()
        {
            _playableGraph = PlayableGraph.Create("MotionMatchingGraph");
            _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            var playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);

            _mixer = AnimationMixerPlayable.Create(_playableGraph, 2);
            playableOutput.SetSourcePlayable(_mixer);

            _playableGraph.Play();
        }

        private void CleanupPlayableGraph()
        {
            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
            }
        }

        private void Update()
        {
            // Tick search interval
            _searchTimer += Time.deltaTime;
            if (_searchTimer >= _updateInterval)
            {
                _searchTimer = 0f;
                // Only run database query if we aren't currently crossfading
                if (_blendTimer == 0f)
                {
                    UpdateMotionMatchingSearch();
                }
            }

            // Tick blend interpolation
            if (_blendTimer > 0f)
            {
                _blendTimer += Time.deltaTime;
                float t = Mathf.Clamp01(_blendTimer / _blendDuration);

                _mixer.SetInputWeight(0, 1f - t);
                _mixer.SetInputWeight(1, t);

                if (t >= 1f)
                {
                    // Swapping playables: Move next playable to current slot, disconnect old
                    _mixer.DisconnectInput(0);
                    _mixer.DisconnectInput(1);

                    if (_currentPlayable.IsValid())
                    {
                        _currentPlayable.Destroy();
                    }

                    _currentPlayable = _nextPlayable;
                    _mixer.ConnectInput(0, _currentPlayable, 0);
                    
                    _mixer.SetInputWeight(0, 1f);
                    _mixer.SetInputWeight(1, 0f);

                    _nextPlayable = default;
                    _blendTimer = 0f;
                }
            }
        }

        private void UpdateMotionMatchingSearch()
        {
            if (_database == null || !_posesNative.IsCreated || _posesNative.Length == 0) return;

            Transform hips = _animator.GetBoneTransform(HumanBodyBones.Hips);
            Transform leftFoot = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform rightFoot = _animator.GetBoneTransform(HumanBodyBones.RightFoot);

            if (hips == null || leftFoot == null || rightFoot == null) return;

            // Extract local bone positions
            float3 currentHipsPos = transform.InverseTransformPoint(hips.position);
            float3 currentLeftFootPos = transform.InverseTransformPoint(leftFoot.position);
            float3 currentRightFootPos = transform.InverseTransformPoint(rightFoot.position);

            // Compute velocities
            float dt = Time.deltaTime > 0f ? Time.deltaTime : 0.033f;
            float3 hipsVel = (currentHipsPos - _prevHipsPos) / dt;
            float3 leftFootVel = (currentLeftFootPos - _prevLeftFootPos) / dt;
            float3 rightFootVel = (currentRightFootPos - _prevRightFootPos) / dt;

            _prevHipsPos = currentHipsPos;
            _prevLeftFootPos = currentLeftFootPos;
            _prevRightFootPos = currentRightFootPos;

            // Extract local query pose & predicted trajectory
            float charFacing = transform.rotation.eulerAngles.y;
            PoseData queryPose = new PoseData
            {
                LeftFootPosition = currentLeftFootPos,
                RightFootPosition = currentRightFootPos,
                HipsPosition = currentHipsPos,

                LeftFootVelocity = leftFootVel,
                RightFootVelocity = rightFootVel,
                HipsVelocity = hipsVel,

                Trajectory0 = TransformTrajectoryToLocal(_trajectoryGenerator.Trajectory0, charFacing),
                Trajectory1 = TransformTrajectoryToLocal(_trajectoryGenerator.Trajectory1, charFacing),
                Trajectory2 = TransformTrajectoryToLocal(_trajectoryGenerator.Trajectory2, charFacing)
            };

            // Calculate expected continuous time in the current clip for continuity penalty
            float currentPlayTime = _currentPlayable.IsValid() ? (float)_currentPlayable.GetTime() : 0f;
            float expectedTime = currentPlayTime + _updateInterval;
            
            Debug.Log($"[MM Trajectory] Query: P0={queryPose.Trajectory0.Position}, P1={queryPose.Trajectory1.Position}, P2={queryPose.Trajectory2.Position}");

            // Setup Burst Job
            var searchJob = new MotionMatchingSearchJob
            {
                Poses = _posesNative,
                QueryPose = queryPose,

                TrajectoryPositionWeight = _trajectoryPositionWeight,
                TrajectoryVelocityWeight = _trajectoryVelocityWeight,
                TrajectoryFacingWeight = _trajectoryFacingWeight,
                PosePositionWeight = _posePositionWeight,
                PoseVelocityWeight = _poseVelocityWeight,

                CurrentClipIndex = _currentClipIndex,
                ExpectedContinuousTime = expectedTime,
                ClipSwitchPenalty = _clipSwitchPenalty,
                TimeJumpPenalty = _timeJumpPenalty,

                BestPoseIndex = _bestPoseIndexOut
            };

            JobHandle handle = searchJob.Schedule();
            handle.Complete();

            int bestPoseIndex = _bestPoseIndexOut[0];
            Debug.Log($"[MM Search] Best Pose Index: {bestPoseIndex}. Total Poses: {_posesNative.Length}");

            if (bestPoseIndex >= 0 && bestPoseIndex < _database.Poses.Length)
            {
                PoseData bestPose = _database.Poses[bestPoseIndex];
                string clipName = (_database.SourceClips != null && bestPose.ClipIndex >= 0 && bestPose.ClipIndex < _database.SourceClips.Length && _database.SourceClips[bestPose.ClipIndex].Clip != null) 
                    ? _database.SourceClips[bestPose.ClipIndex].Clip.name 
                    : $"Clip {bestPose.ClipIndex}";
                
                // If it is a different clip or we have jumped in time significantly, initiate blend
                bool isDifferentClip = bestPose.ClipIndex != _currentClipIndex;
                bool isTimeJump = !isDifferentClip && Mathf.Abs(bestPose.Time - currentPlayTime) > 0.15f;

                Debug.Log($"[MM Search] Best: {clipName} at t={bestPose.Time:F2}s. Current: ClipIndex={_currentClipIndex}, t={currentPlayTime:F2}s. isDiffClip={isDifferentClip}, isJump={isTimeJump}");

                if (isDifferentClip || isTimeJump)
                {
                    Debug.Log($"[MM Search] Transitioning from {_currentClipIndex} to {bestPose.ClipIndex} (startTime: {bestPose.Time:F2}s)");
                    TransitionToPose(bestPose.ClipIndex, bestPose.Time);
                }
            }
        }

        private TrajectoryPointData TransformTrajectoryToLocal(TrajectoryPointData worldPt, float characterFacingAngle)
        {
            float3 localPos = transform.InverseTransformPoint(worldPt.Position);
            localPos.y = 0f; // Force Y to 0 for horizontal ground projection

            float3 localVel = transform.InverseTransformDirection(worldPt.Velocity);
            localVel.y = 0f; // Force Y to 0

            return new TrajectoryPointData
            {
                Position = localPos,
                Velocity = localVel,
                FacingAngle = Mathf.DeltaAngle(characterFacingAngle, worldPt.FacingAngle)
            };
        }

        private void TransitionToPose(int clipIndex, float startTime)
        {
            if (_database.SourceClips == null || clipIndex < 0 || clipIndex >= _database.SourceClips.Length) return;
            AnimationClip clip = _database.SourceClips[clipIndex].Clip;
            if (clip == null) return;

            var newPlayable = AnimationClipPlayable.Create(_playableGraph, clip);
            newPlayable.SetTime(startTime);

            if (!_currentPlayable.IsValid())
            {
                // Immediate play
                _currentPlayable = newPlayable;
                _mixer.ConnectInput(0, _currentPlayable, 0);
                _mixer.SetInputWeight(0, 1f);
                _mixer.SetInputWeight(1, 0f);
                _currentClipIndex = clipIndex;
            }
            else
            {
                // Crossfade
                if (_nextPlayable.IsValid())
                {
                    _mixer.DisconnectInput(1);
                    _nextPlayable.Destroy();
                }

                _nextPlayable = newPlayable;
                _mixer.ConnectInput(1, _nextPlayable, 0);
                _mixer.SetInputWeight(0, 1f);
                _mixer.SetInputWeight(1, 0f);
                
                _currentClipIndex = clipIndex;
                _blendTimer = 0.001f; // Starts the crossfade ticking in Update()
            }
        }
    }

    /// <summary>
    /// Burst-compiled job to evaluate the cost of every frame in the Pose Database.
    /// </summary>
    [Unity.Burst.BurstCompile]
    public struct MotionMatchingSearchJob : IJob
    {
        [ReadOnly] public NativeArray<PoseData> Poses;
        public PoseData QueryPose;

        // Weights
        public float TrajectoryPositionWeight;
        public float TrajectoryVelocityWeight;
        public float TrajectoryFacingWeight;
        public float PosePositionWeight;
        public float PoseVelocityWeight;

        // Penalties/Biases
        public int CurrentClipIndex;
        public float ExpectedContinuousTime;
        public float ClipSwitchPenalty;
        public float TimeJumpPenalty;

        [WriteOnly] public NativeArray<int> BestPoseIndex;

        public void Execute()
        {
            float minCost = float.MaxValue;
            int bestIndex = -1;

            for (int i = 0; i < Poses.Length; i++)
            {
                PoseData pose = Poses[i];

                // 1. Trajectory Cost
                float trajCost = 0f;

                // Point 0 (0.33s)
                trajCost += math.distancesq(pose.Trajectory0.Position, QueryPose.Trajectory0.Position) * TrajectoryPositionWeight;
                trajCost += math.distancesq(pose.Trajectory0.Velocity, QueryPose.Trajectory0.Velocity) * TrajectoryVelocityWeight;
                float df0 = math.abs(DeltaAngle(pose.Trajectory0.FacingAngle, QueryPose.Trajectory0.FacingAngle));
                trajCost += df0 * df0 * TrajectoryFacingWeight;

                // Point 1 (0.66s)
                trajCost += math.distancesq(pose.Trajectory1.Position, QueryPose.Trajectory1.Position) * TrajectoryPositionWeight;
                trajCost += math.distancesq(pose.Trajectory1.Velocity, QueryPose.Trajectory1.Velocity) * TrajectoryVelocityWeight;
                float df1 = math.abs(DeltaAngle(pose.Trajectory1.FacingAngle, QueryPose.Trajectory1.FacingAngle));
                trajCost += df1 * df1 * TrajectoryFacingWeight;

                // Point 2 (1.0s)
                trajCost += math.distancesq(pose.Trajectory2.Position, QueryPose.Trajectory2.Position) * TrajectoryPositionWeight;
                trajCost += math.distancesq(pose.Trajectory2.Velocity, QueryPose.Trajectory2.Velocity) * TrajectoryVelocityWeight;
                float df2 = math.abs(DeltaAngle(pose.Trajectory2.FacingAngle, QueryPose.Trajectory2.FacingAngle));
                trajCost += df2 * df2 * TrajectoryFacingWeight;

                // 2. Pose Cost
                float poseCost = 0f;
                poseCost += math.distancesq(pose.LeftFootPosition, QueryPose.LeftFootPosition) * PosePositionWeight;
                poseCost += math.distancesq(pose.RightFootPosition, QueryPose.RightFootPosition) * PosePositionWeight;
                poseCost += math.distancesq(pose.HipsPosition, QueryPose.HipsPosition) * PosePositionWeight;

                poseCost += math.distancesq(pose.LeftFootVelocity, QueryPose.LeftFootVelocity) * PoseVelocityWeight;
                poseCost += math.distancesq(pose.RightFootVelocity, QueryPose.RightFootVelocity) * PoseVelocityWeight;
                poseCost += math.distancesq(pose.HipsVelocity, QueryPose.HipsVelocity) * PoseVelocityWeight;

                // 3. Biases & Penalties
                float bias = 0f;
                if (pose.ClipIndex != CurrentClipIndex)
                {
                    bias += ClipSwitchPenalty;
                }
                else
                {
                    // Check if it's a jump in time within the current playing clip
                    float timeDiff = math.abs(pose.Time - ExpectedContinuousTime);
                    if (timeDiff > 0.05f)
                    {
                        bias += TimeJumpPenalty;
                    }
                }

                float totalCost = trajCost + poseCost + bias;
                if (totalCost < minCost)
                {
                    minCost = totalCost;
                    bestIndex = i;
                }
            }

            BestPoseIndex[0] = bestIndex;
        }

        // Burst-compatible angle difference in degrees
        private static float DeltaAngle(float current, float target)
        {
            float delta = (target - current) % 360f;
            if (delta > 180f) delta -= 360f;
            else if (delta < -180f) delta += 360f;
            return delta;
        }
    }
}
