using UnityEngine;

namespace Systems.Audio
{
    [RequireComponent(typeof(AudioListener))]
    public class AcousticReverbEstimator : MonoBehaviour, IAudioRaycastReceiver
    {
        [Header("References")]
        [Tooltip("The material mapping database. If null, default absorption values are used.")]
        public AcousticPhysicMaterialMap MaterialMap;

        [Header("Layer Configuration")]
        [Tooltip("Override layer mask for reverb raycasts. If -1, uses default mask from AudioRaycastManager.")]
        public LayerMask ReverbLayerMask = -1;

        [Header("Raycast Settings")]
        [Tooltip("Number of rays to cast in a spherical distribution.")]
        [Range(8, 128)]
        public int RayCount = 32;

        [Tooltip("Maximum distance of the rays per bounce (meters).")]
        public float MaxRayDistance = 30f;

        [Tooltip("Maximum number of reflection bounces for each ray to map the room.")]
        [Range(1, 16)]
        public int MaxBounces = 8;

        [Tooltip("How often to start a new multi-bounce reverb scan in seconds.")]
        [Range(0.05f, 2f)]
        public float UpdateInterval = 0.25f;

        [Tooltip("Speed at which reverb filter parameters fade to their targets (to prevent audio popping).")]
        public float FadeSpeed = 2f;

        [Header("Reverb Mapping Limits")]
        [Tooltip("Minimum decay time in seconds for small/dead spaces.")]
        public float MinDecayTime = 0.1f;

        [Tooltip("Maximum decay time in seconds for massive/reflective spaces.")]
        public float MaxDecayTime = 7.0f;

        [Tooltip("Default absorption coefficient for missed rays (open space/sky). 1.0 means sound escaping to infinity.")]
        [Range(0f, 1f)]
        public float OpenSpaceAbsorption = 1f;

        private AudioReverbFilter _reverbFilter;
        private float _nextUpdateTime;
        private Transform _listenerRoot;

        private void FindRoot()
        {
            if (_listenerRoot == null)
            {
                var rb = GetComponentInParent<Rigidbody>();
                _listenerRoot = rb != null ? rb.transform : transform;
            }
        }

        // Structures to track active rays during multi-bounce scanning
        private struct ActiveRay
        {
            public Vector3 Position;
            public Vector3 Direction;
            public float Energy;
            public bool IsActive;
        }

        private ActiveRay[] _activeRays;
        private bool _isScanning;
        private int _currentBounceIndex;
        private int _pendingRaycastCount;

        // Buffers to accumulate statistics across all bounces
        private float[] _accumulatedDistances;
        private float[] _accumulatedAbsorptions;
        private float[] _accumulatedCutoffs;
        private int _totalRecordedHits;

        // Pre-allocated array to store zig-zag path points for Scene view rendering
        private Vector3[] _rayPathPoints;
        private int[] _rayPathPointsCount;

        // Target values for smoothing
        private float _targetDecayTime = 1f;
        private float _targetReverbDelay = 0.04f;
        private float _targetRoomHF = 0f;

        // Cache hit distances for Gizmos rendering
        private float[] _gizmoHitDistances;
        private float[] _gizmoHitAbsorptions;

        private void Awake()
        {
            // Check if there is another AudioListener in the scene that is not on this GameObject
            AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
            AudioListener otherListener = null;
            foreach (var listener in listeners)
            {
                if (listener.gameObject != this.gameObject && listener.enabled)
                {
                    otherListener = listener;
                    break;
                }
            }

            if (otherListener != null)
            {
                // We have a conflict! The camera/other object should be the sole AudioListener.
                // Disable the AudioListener on this GameObject
                var ourListener = GetComponent<AudioListener>();
                if (ourListener != null)
                {
                    ourListener.enabled = false;
                }

                // Ensure the other AudioListener has AcousticReverbEstimator
                if (otherListener.GetComponent<AcousticReverbEstimator>() == null)
                {
                    var newEstimator = otherListener.gameObject.AddComponent<AcousticReverbEstimator>();
                    newEstimator.MaterialMap = this.MaterialMap;
                    newEstimator.ReverbLayerMask = this.ReverbLayerMask;
                    newEstimator.RayCount = this.RayCount;
                    newEstimator.MaxRayDistance = this.MaxRayDistance;
                    newEstimator.MaxBounces = this.MaxBounces;
                    newEstimator.UpdateInterval = this.UpdateInterval;
                    newEstimator.FadeSpeed = this.FadeSpeed;
                    newEstimator.MinDecayTime = this.MinDecayTime;
                    newEstimator.MaxDecayTime = this.MaxDecayTime;
                    newEstimator.OpenSpaceAbsorption = this.OpenSpaceAbsorption;
                }

                // Disable this component since we migrated its functionality
                enabled = false;
                return;
            }

            _reverbFilter = GetComponent<AudioReverbFilter>();
            if (_reverbFilter == null)
            {
                _reverbFilter = gameObject.AddComponent<AudioReverbFilter>();
            }

            InitializeArrays();
        }

        private void InitializeArrays()
        {
            _activeRays = new ActiveRay[RayCount];
            
            // Reverb statistics buffers (Max allocation = RayCount * MaxBounces)
            int maxTotalHits = RayCount * MaxBounces;
            _accumulatedDistances = new float[maxTotalHits];
            _accumulatedAbsorptions = new float[maxTotalHits];
            _accumulatedCutoffs = new float[maxTotalHits];

            // Path points buffer for visual debug lines (Size: RayCount * (MaxBounces + 1))
            _rayPathPoints = new Vector3[RayCount * (MaxBounces + 1)];
            _rayPathPointsCount = new int[RayCount];

            // Gizmos cache
            _gizmoHitDistances = new float[RayCount];
            _gizmoHitAbsorptions = new float[RayCount];
        }

        private void Start()
        {
            _nextUpdateTime = Time.time + UnityEngine.Random.Range(0f, UpdateInterval);
        }

        private void Update()
        {
            // Smoothly interpolate the reverb filter parameters to avoid pops
            _reverbFilter.decayTime = Mathf.MoveTowards(_reverbFilter.decayTime, _targetDecayTime, Time.deltaTime * FadeSpeed);
            _reverbFilter.reverbDelay = Mathf.MoveTowards(_reverbFilter.reverbDelay, _targetReverbDelay, Time.deltaTime * FadeSpeed * 0.1f);
            _reverbFilter.roomHF = Mathf.MoveTowards(_reverbFilter.roomHF, _targetRoomHF, Time.deltaTime * FadeSpeed * 1000f);

            // Re-allocate arrays if settings were changed at runtime in editor
            if (_activeRays.Length != RayCount || _rayPathPoints.Length != RayCount * (MaxBounces + 1))
            {
                InitializeArrays();
            }

            if (Time.time >= _nextUpdateTime && !_isScanning)
            {
                _nextUpdateTime = Time.time + UpdateInterval;
                StartMultiBounceScan();
            }
        }

        private void StartMultiBounceScan()
        {
            _isScanning = true;
            _currentBounceIndex = 0;
            _totalRecordedHits = 0;
            _pendingRaycastCount = RayCount;

            Vector3 origin = transform.position;
            float goldenRatioAngle = 2.39996323f;

            for (int i = 0; i < RayCount; i++)
            {
                // Fibonacci Sphere directions
                float t = (float)i / RayCount;
                float phi = Mathf.Acos(1f - 2f * (i + 0.5f) / RayCount);
                float theta = goldenRatioAngle * i;

                Vector3 direction = new Vector3(
                    Mathf.Cos(theta) * Mathf.Sin(phi),
                    Mathf.Sin(theta) * Mathf.Sin(phi),
                    Mathf.Cos(phi)
                );

                _activeRays[i] = new ActiveRay
                {
                    Position = origin,
                    Direction = direction,
                    Energy = 1f,
                    IsActive = true
                };

                // Record start point for Gizmos
                _rayPathPoints[i * (MaxBounces + 1)] = origin;
                _rayPathPointsCount[i] = 1;

                // Queue first bounce (ID: i)
                AudioRaycastManager.Instance.QueueRaycast(
                    origin,
                    direction,
                    MaxRayDistance,
                    this,
                    i, // ID is the ray index
                    ReverbLayerMask
                );
            }
        }

        public void OnRaycastComplete(RaycastHit hit, int requestId)
        {
            if (!_isScanning) return;

            int rayIndex = requestId;
            if (rayIndex < 0 || rayIndex >= RayCount) return;

            if (_activeRays[rayIndex].IsActive)
            {
                FindRoot();
                // If the ray hits the listener's own body collider, treat it as a miss (open air) to avoid self-reverb
                if (hit.collider != null && hit.collider.transform.IsChildOf(_listenerRoot))
                {
                    hit = default;
                }
                ProcessBounceResult(rayIndex, hit);
            }

            _pendingRaycastCount--;
            if (_pendingRaycastCount == 0)
            {
                EvaluateBounceStep();
            }
        }

        private void ProcessBounceResult(int rayIndex, RaycastHit hit)
        {
            ActiveRay ray = _activeRays[rayIndex];
            int pathIndex = rayIndex * (MaxBounces + 1);
            int currentPoints = _rayPathPointsCount[rayIndex];

            if (hit.collider != null)
            {
                // Record statistics
                int statsIndex = _totalRecordedHits;
                _accumulatedDistances[statsIndex] = hit.distance;
                
                PhysicsMaterial physicsMaterial = hit.collider.sharedMaterial;
                AudioAcousticMaterial acousticMaterial = MaterialMap != null ? MaterialMap.GetAcousticMaterial(physicsMaterial) : null;

                float absorption = 0.1f;
                float cutoff = 5000f;

                if (acousticMaterial != null)
                {
                    absorption = acousticMaterial.AbsorptionCoefficient;
                    cutoff = acousticMaterial.LowPassCutoffHz;
                }

                _accumulatedAbsorptions[statsIndex] = absorption;
                _accumulatedCutoffs[statsIndex] = cutoff;
                _totalRecordedHits++;

                // Calculate bounce reflection
                // ray.Direction and hit.normal are already normalized, so reflectDir is naturally normalized
                Vector3 reflectDir = Vector3.Reflect(ray.Direction, hit.normal);
                
                // Record path point for Gizmos
                if (currentPoints < MaxBounces + 1)
                {
                    _rayPathPoints[pathIndex + currentPoints] = hit.point;
                    _rayPathPointsCount[rayIndex]++;
                }

                // Update active ray state for the next bounce
                _activeRays[rayIndex].Position = hit.point + reflectDir * 0.02f; // Offset to prevent self-intersection
                _activeRays[rayIndex].Direction = reflectDir;
                _activeRays[rayIndex].Energy *= (1f - absorption);

                // Cache first hit details for fallback/basic gizmos
                if (_currentBounceIndex == 0)
                {
                    _gizmoHitDistances[rayIndex] = hit.distance;
                    _gizmoHitAbsorptions[rayIndex] = absorption;
                }
            }
            else
            {
                // Ray missed (sound escaped to open space). Terminate the path.
                int statsIndex = _totalRecordedHits;
                _accumulatedDistances[statsIndex] = MaxRayDistance;
                _accumulatedAbsorptions[statsIndex] = OpenSpaceAbsorption;
                _accumulatedCutoffs[statsIndex] = 20000f;
                _totalRecordedHits++;

                // Record end point for Gizmos
                if (currentPoints < MaxBounces + 1)
                {
                    _rayPathPoints[pathIndex + currentPoints] = ray.Position + ray.Direction * MaxRayDistance;
                    _rayPathPointsCount[rayIndex]++;
                }

                _activeRays[rayIndex].IsActive = false;
                _activeRays[rayIndex].Energy = 0f;

                if (_currentBounceIndex == 0)
                {
                    _gizmoHitDistances[rayIndex] = MaxRayDistance;
                    _gizmoHitAbsorptions[rayIndex] = OpenSpaceAbsorption;
                }
            }
        }

        private void EvaluateBounceStep()
        {
            _currentBounceIndex++;

            if (_currentBounceIndex >= MaxBounces)
            {
                FinalizeScanCalculations();
                return;
            }

            // Check how many rays are still active
            int nextActiveCount = 0;
            for (int i = 0; i < RayCount; i++)
            {
                if (_activeRays[i].IsActive && _activeRays[i].Energy > 0.02f)
                {
                    nextActiveCount++;
                }
                else
                {
                    _activeRays[i].IsActive = false;
                }
            }

            if (nextActiveCount == 0)
            {
                FinalizeScanCalculations();
                return;
            }

            _pendingRaycastCount = nextActiveCount;

            // Queue raycasts for all active rays
            for (int i = 0; i < RayCount; i++)
            {
                if (_activeRays[i].IsActive)
                {
                    AudioRaycastManager.Instance.QueueRaycast(
                        _activeRays[i].Position,
                        _activeRays[i].Direction,
                        MaxRayDistance,
                        this,
                        i,
                        ReverbLayerMask
                    );
                }
            }
        }

        private void FinalizeScanCalculations()
        {
            _isScanning = false;

            if (_totalRecordedHits == 0) return;

            float sumDistance = 0f;
            float sumAbsorption = 0f;
            float sumCutoff = 0f;

            for (int i = 0; i < _totalRecordedHits; i++)
            {
                sumDistance += _accumulatedDistances[i];
                sumAbsorption += _accumulatedAbsorptions[i];
                sumCutoff += _accumulatedCutoffs[i];
            }

            float avgDistance = sumDistance / _totalRecordedHits;
            float avgAbsorption = sumAbsorption / _totalRecordedHits;
            float avgCutoff = sumCutoff / _totalRecordedHits;

            // 1. Eyring Formula for RT60 (reverberation decay time in seconds)
            _targetDecayTime = AcousticMath.CalculateEyringReverbDecay(avgDistance, avgAbsorption, MinDecayTime, MaxDecayTime);

            // 2. Reverb Delay
            _targetReverbDelay = AcousticMath.CalculateReverbDelay(avgDistance);

            // 3. High Frequency Damping (roomHF)
            _targetRoomHF = AcousticMath.CalculateRoomHF(avgCutoff);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            // Draw full multi-bounce zig-zag paths
            for (int i = 0; i < RayCount; i++)
            {
                int pathIndex = i * (MaxBounces + 1);
                int pointsCount = _rayPathPointsCount[i];

                if (pointsCount < 2) continue;

                // Determine base color from first hit's absorption
                float absorption = i < _gizmoHitAbsorptions.Length ? _gizmoHitAbsorptions[i] : 0.1f;
                Color rayColor = Color.Lerp(Color.red, Color.cyan, absorption);

                for (int j = 0; j < pointsCount - 1; j++)
                {
                    Vector3 start = _rayPathPoints[pathIndex + j];
                    Vector3 end = _rayPathPoints[pathIndex + j + 1];

                    // Fade color slightly for later bounces to visualize decay
                    float fadeFactor = 1f - ((float)j / MaxBounces) * 0.7f;
                    Gizmos.color = new Color(rayColor.r, rayColor.g, rayColor.b, fadeFactor);

                    Gizmos.DrawLine(start, end);
                    Gizmos.DrawWireSphere(end, 0.1f * fadeFactor);
                }
            }
        }
#endif
    }
}
