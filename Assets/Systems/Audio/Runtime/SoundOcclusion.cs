using UnityEngine;

namespace Systems.Audio
{
    public enum OcclusionSampleMode
    {
        SingleRay = 1,
        ThreeRays = 3,
        FiveRays = 5
    }

    [RequireComponent(typeof(AudioSource))]
    public class SoundOcclusion : MonoBehaviour, IAudioRaycastReceiver
    {
        [Header("References")]
        [Tooltip("The material mapping database. If null, default occlusion parameters will be used.")]
        public AcousticPhysicMaterialMap MaterialMap;

        [Header("Layer Configuration")]
        [Tooltip("Override layer mask for occlusion raycasts. If -1, uses default mask from AudioRaycastManager.")]
        public LayerMask OcclusionLayerMask = -1;

        [Header("Occlusion Sample Settings")]
        [Tooltip("Number of rays to cast to estimate occlusion. Multi-ray (3 or 5) approximates sound diffraction around doorways and corners.")]
        public OcclusionSampleMode SampleMode = OcclusionSampleMode.ThreeRays;

        [Tooltip("The width (meters) of the ray spread. Higher values make diffraction transitions wider and smoother.")]
        public float SpreadWidth = 1.5f;

        [Header("Occlusion Timing")]
        [Tooltip("How often to check for occlusion in seconds. Staggering checks saves performance.")]
        [Range(0.01f, 1f)]
        public float UpdateInterval = 0.1f;

        [Tooltip("Speed at which volume fades to the target occlusion level (to prevent audio popping).")]
        public float VolumeFadeSpeed = 5f;

        [Tooltip("Speed at which the low-pass filter cutoff frequency fades to the target frequency.")]
        public float CutoffFadeSpeed = 5f;

        [Tooltip("Maximum thickness of walls in meters. Sound is fully occluded at this thickness.")]
        public float MaxThicknessThreshold = 5f;

        [Tooltip("Minimum low-pass cutoff frequency in Hz when fully occluded.")]
        [Range(10f, 2000f)]
        public float MinCutoffFrequency = 150f;

        [Tooltip("Maximum low-pass cutoff frequency in Hz when fully clear.")]
        [Range(2000f, 22000f)]
        public float MaxCutoffFrequency = 22000f;

        [Header("Distance Attenuation")]
        [Tooltip("If true, automatically sets the AudioSource's spatial blend to 3D (1.0) on startup.")]
        public bool Force3DSpatialization = true;

        [Tooltip("Minimum distance for 3D spatialization. Sound is at full volume within this distance.")]
        public float MinDistance = 1f;

        [Tooltip("Maximum distance for 3D spatialization. Sound is completely silent beyond this distance.")]
        public float MaxDistance = 50f;

        [Tooltip("Rolloff mode to use for 3D spatialization.")]
        public AudioRolloffMode RolloffMode = AudioRolloffMode.Linear;

        [Tooltip("Enable distance-based low-pass filtering (air absorption) to muffle sound over distance.")]
        public bool EnableAirAbsorption = true;

        [Tooltip("How many Hz of high-frequency detail are lost per meter of distance. (e.g. 150Hz/m means sound is heavily muffled at 100m).")]
        public float AirAbsorptionRate = 150f;

        private AudioSource _audioSource;
        private AudioLowPassFilter _lowPassFilter;
        private AudioListener _cachedListener;
        private Transform _sourceRoot;
        private Transform _listenerRoot;

        private void FindRoots()
        {
            if (_sourceRoot == null)
            {
                var rb = GetComponentInParent<Rigidbody>();
                _sourceRoot = rb != null ? rb.transform : transform;
            }

            if (_listenerRoot == null && _cachedListener != null)
            {
                var rb = _cachedListener.GetComponentInParent<Rigidbody>();
                _listenerRoot = rb != null ? rb.transform : _cachedListener.transform;
            }
        }

        private float _baseVolume = 1f;
        private float _targetVolumeScale = 1f;
        private float _targetCutoffHz = 22000f;
        private float _nextUpdateTime;

        // Structure to track each sample ray's state asynchronously
        private struct RaySample
        {
            public bool ForwardPending;
            public bool BackwardPending;
            public Vector3 ForwardHitPoint;
            public AudioAcousticMaterial HitMaterial;
            public float CalculatedVolumeScale;
            public float CalculatedCutoffHz;
            public float LastValidVolumeScale;
            public float LastValidCutoffHz;
        }

        private RaySample[] _samples = new RaySample[5];
        private int _activeSampleCount;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _baseVolume = _audioSource.volume;

            if (MaterialMap == null)
            {
                MaterialMap = Resources.Load<AcousticPhysicMaterialMap>("AcousticPhysicMaterialMap");
            }

            if (Force3DSpatialization)
            {
                _audioSource.spatialBlend = 1f;
                _audioSource.minDistance = MinDistance;
                _audioSource.maxDistance = MaxDistance;
                _audioSource.rolloffMode = RolloffMode;
            }

            // Ensure we have a low-pass filter attached
            _lowPassFilter = GetComponent<AudioLowPassFilter>();
            if (_lowPassFilter == null)
            {
                _lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
            }
            _lowPassFilter.cutoffFrequency = MaxCutoffFrequency;
        }

        private void Start()
        {
            FindListener();
            _nextUpdateTime = Time.time + UnityEngine.Random.Range(0f, UpdateInterval); // Stagger initial start times

            for (int i = 0; i < _samples.Length; i++)
            {
                _samples[i].LastValidVolumeScale = 1f;
                _samples[i].LastValidCutoffHz = MaxCutoffFrequency;
            }
        }

        private void OnEnable()
        {
            _targetVolumeScale = 1f;
            _targetCutoffHz = MaxCutoffFrequency;
            if (_lowPassFilter != null) _lowPassFilter.cutoffFrequency = MaxCutoffFrequency;
        }

        private void Update()
        {
            // Smoothly interpolate volume and low-pass cutoff to prevent audio pops/clicks
            float currentTargetVolume = _baseVolume * _targetVolumeScale;
            _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, currentTargetVolume, Time.deltaTime * VolumeFadeSpeed * _baseVolume);
            
            float targetCutoff = _targetCutoffHz;
            if (EnableAirAbsorption && _cachedListener != null)
            {
                float distance = Vector3.Distance(transform.position, _cachedListener.transform.position);
                float airAbsorptionCutoff = MaxCutoffFrequency - (distance * AirAbsorptionRate);
                targetCutoff = Mathf.Min(targetCutoff, Mathf.Max(airAbsorptionCutoff, MinCutoffFrequency));
            }

            if (_lowPassFilter != null)
            {
                _lowPassFilter.cutoffFrequency = Mathf.MoveTowards(_lowPassFilter.cutoffFrequency, targetCutoff, Time.deltaTime * CutoffFadeSpeed * (MaxCutoffFrequency - MinCutoffFrequency));
            }

            // Check if it's time to query the physics system
            if (Time.time >= _nextUpdateTime)
            {
                _nextUpdateTime = Time.time + UpdateInterval;
                ExecuteOcclusionCheck();
            }
        }

        private void FindListener()
        {
            if (_cachedListener == null)
            {
                _cachedListener = FindFirstObjectByType<AudioListener>();
            }
        }

        private void ExecuteOcclusionCheck()
        {
            FindListener();

            if (_cachedListener == null || !_audioSource.isPlaying)
            {
                return;
            }

            // If we still have pending rays from previous ticks, skip this update to avoid queue overflows
            if (IsBatchPending())
            {
                return;
            }

            Vector3 sourcePos = transform.position;
            Vector3 listenerPos = _cachedListener.transform.position;
            Vector3 toListener = listenerPos - sourcePos;
            float sqrDistance = toListener.sqrMagnitude;

            if (sqrDistance <= 0.0001f) // 0.01f * 0.01f
            {
                ClearOcclusion();
                return;
            }

            float distance = Mathf.Sqrt(sqrDistance);
            _activeSampleCount = (int)SampleMode;
            Vector3 direction = toListener / distance;
            // BOLT OPTIMIZATION: rightNorm is normalized. Cross product of two normalized, perpendicular vectors (direction and rightNorm) is inherently normalized.
            // Avoids calling .normalized on the second cross product and avoids multiplying by SpreadWidth twice.
            Vector3 rightNorm = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 upNorm = Vector3.Cross(direction, rightNorm);
            Vector3 right = rightNorm * SpreadWidth;
            Vector3 up = upNorm * SpreadWidth;

            for (int i = 0; i < _activeSampleCount; i++)
            {
                // Reset sample state
                _samples[i].ForwardPending = true;
                _samples[i].BackwardPending = false;
                _samples[i].CalculatedVolumeScale = 1f;
                _samples[i].CalculatedCutoffHz = MaxCutoffFrequency;

                // Determine start/end point offsets
                Vector3 offset = Vector3.zero;
                switch (i)
                {
                    case 1: offset = -right; break; // Left ray
                    case 2: offset = right; break;  // Right ray
                    case 3: offset = up; break;     // Up ray
                    case 4: offset = -up; break;    // Down ray
                }

                Vector3 start = sourcePos + offset;
                Vector3 end = listenerPos + offset;
                Vector3 sampleToListener = end - start;

                float sampleSqrDistance = sampleToListener.sqrMagnitude;
                float sampleDistance = sampleSqrDistance > 0.00001f ? Mathf.Sqrt(sampleSqrDistance) : 0f;
                Vector3 sampleDir = sampleDistance > 0f ? sampleToListener / sampleDistance : Vector3.forward;

                // Queue forward ray (Request ID: i)
                AudioRaycastManager.Instance.QueueRaycast(
                    start,
                    sampleDir,
                    sampleDistance,
                    this,
                    i, // ID is the sample index (0 to 4)
                    OcclusionLayerMask
                );
            }
        }

        private bool IsBatchPending()
        {
            for (int i = 0; i < _activeSampleCount; i++)
            {
                if (_samples[i].ForwardPending || _samples[i].BackwardPending)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnRaycastComplete(RaycastHit hit, int requestId)
        {
            // IDs below 10 are forward rays: ID = sampleIndex
            if (requestId < 10)
            {
                int index = requestId;
                if (index < 0 || index >= _activeSampleCount) return;

                _samples[index].ForwardPending = false;
                ProcessSampleForwardHit(index, hit);
            }
            // IDs >= 10 are backward rays: ID = 10 + sampleIndex
            else
            {
                int index = requestId - 10;
                if (index < 0 || index >= _activeSampleCount) return;

                _samples[index].BackwardPending = false;
                ProcessSampleBackwardHit(index, hit);
            }

            // Once the entire batch of rays is complete, average the results
            if (!IsBatchPending())
            {
                EvaluateBatchResults();
            }
        }

        private void ProcessSampleForwardHit(int index, RaycastHit hit)
        {
            FindRoots();
            if (hit.collider == null || 
                hit.collider.transform.IsChildOf(_sourceRoot) || 
                (_listenerRoot != null && hit.collider.transform.IsChildOf(_listenerRoot)))
            {
                // Clear path: full volume and brightness for this sample
                _samples[index].CalculatedVolumeScale = 1f;
                _samples[index].CalculatedCutoffHz = MaxCutoffFrequency;
                _samples[index].LastValidVolumeScale = 1f;
                _samples[index].LastValidCutoffHz = MaxCutoffFrequency;
                return;
            }

            // Hit an obstacle. Save the entry point and material
            _samples[index].ForwardHitPoint = hit.point;
            
            PhysicsMaterial physicsMaterial = hit.collider.sharedMaterial;
            _samples[index].HitMaterial = MaterialMap != null ? MaterialMap.GetAcousticMaterial(physicsMaterial) : null;

            // Set a default muffle fallback (assuming 0.5m wall) in case the backward ray fails to find the exit point
            float transmissionLossDb = _samples[index].HitMaterial != null ? _samples[index].HitMaterial.TransmissionLossDb : 15f;
            float materialCutoff = _samples[index].HitMaterial != null ? _samples[index].HitMaterial.LowPassCutoffHz : 800f;
            float fallbackLoss = AcousticMath.CalculateTransmissionLossDb(transmissionLossDb, 0.5f);
            _samples[index].CalculatedVolumeScale = AcousticMath.CalculateVolumeScaleFromDb(fallbackLoss);
            _samples[index].CalculatedCutoffHz = AcousticMath.CalculateCutoffFrequency(0.5f, MaxThicknessThreshold, materialCutoff, MinCutoffFrequency, MaxCutoffFrequency);

            if (_cachedListener == null)
            {
                return;
            }

            // Setup backward ray for this sample
            Vector3 offset = Vector3.zero;
            Vector3 toListenerVec = _cachedListener.transform.position - transform.position;
            float toListenerDistance = Mathf.Sqrt(toListenerVec.sqrMagnitude);
            Vector3 direction = toListenerDistance > 0.00001f ? toListenerVec / toListenerDistance : Vector3.forward;
            // BOLT OPTIMIZATION: rightNorm is normalized. Cross product of two normalized, perpendicular vectors (direction and rightNorm) is inherently normalized.
            // Avoids calling .normalized on the second cross product and avoids multiplying by SpreadWidth twice.
            Vector3 rightNorm = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 upNorm = Vector3.Cross(direction, rightNorm);
            Vector3 right = rightNorm * SpreadWidth;
            Vector3 up = upNorm * SpreadWidth;

            switch (index)
            {
                case 1: offset = -right; break;
                case 2: offset = right; break;
                case 3: offset = up; break;
                case 4: offset = -up; break;
            }

            Vector3 start = _cachedListener.transform.position + offset;
            Vector3 end = transform.position + offset;
            Vector3 sampleToSource = end - start;

            float sampleSqrDistance = sampleToSource.sqrMagnitude;
            float sampleDistance = sampleSqrDistance > 0.00001f ? Mathf.Sqrt(sampleSqrDistance) : 0f;
            Vector3 sampleDir = sampleDistance > 0f ? sampleToSource / sampleDistance : Vector3.forward;

            _samples[index].BackwardPending = true;
            AudioRaycastManager.Instance.QueueRaycast(
                start,
                sampleDir,
                sampleDistance,
                this,
                10 + index, // ID encodes backward ray for this sample index
                OcclusionLayerMask
            );
        }

        private void ProcessSampleBackwardHit(int index, RaycastHit hit)
        {
            FindRoots();
            if (hit.collider == null || 
                hit.collider.transform.IsChildOf(_sourceRoot) || 
                (_listenerRoot != null && hit.collider.transform.IsChildOf(_listenerRoot)))
            {
                // Backward ray failed - reuse last valid values if we have them
                if (_samples[index].LastValidCutoffHz > 0f)
                {
                    _samples[index].CalculatedVolumeScale = _samples[index].LastValidVolumeScale;
                    _samples[index].CalculatedCutoffHz = _samples[index].LastValidCutoffHz;
                }
                return;
            }

            Vector3 backwardHitPoint = hit.point;
            float sqrThickness = (_samples[index].ForwardHitPoint - backwardHitPoint).sqrMagnitude;
            float thickness = sqrThickness > MaxThicknessThreshold * MaxThicknessThreshold
                ? MaxThicknessThreshold
                : Mathf.Sqrt(sqrThickness);

            float transmissionLossDb = 15f; // Fallback
            float materialCutoff = 800f;   // Fallback

            if (_samples[index].HitMaterial != null)
            {
                transmissionLossDb = _samples[index].HitMaterial.TransmissionLossDb;
                materialCutoff = _samples[index].HitMaterial.LowPassCutoffHz;
            }

            float totalLossDb = AcousticMath.CalculateTransmissionLossDb(transmissionLossDb, thickness);
            _samples[index].CalculatedVolumeScale = AcousticMath.CalculateVolumeScaleFromDb(totalLossDb);
            _samples[index].CalculatedCutoffHz = AcousticMath.CalculateCutoffFrequency(
                thickness,
                MaxThicknessThreshold,
                materialCutoff,
                MinCutoffFrequency,
                MaxCutoffFrequency
            );

            // Update last valid values
            _samples[index].LastValidVolumeScale = _samples[index].CalculatedVolumeScale;
            _samples[index].LastValidCutoffHz = _samples[index].CalculatedCutoffHz;
        }

        private void EvaluateBatchResults()
        {
            float sumVolume = 0f;
            float sumCutoff = 0f;

            for (int i = 0; i < _activeSampleCount; i++)
            {
                sumVolume += _samples[i].CalculatedVolumeScale;
                sumCutoff += _samples[i].CalculatedCutoffHz;
            }

            float avgVolume = sumVolume / _activeSampleCount;
            float avgCutoff = sumCutoff / _activeSampleCount;

            // Smooth the targets to prevent high-frequency physics/animation jitter from causing audio warbling
            _targetVolumeScale = Mathf.Lerp(_targetVolumeScale, avgVolume, 0.4f);
            _targetCutoffHz = Mathf.Lerp(_targetCutoffHz, avgCutoff, 0.4f);
        }

        private void ClearOcclusion()
        {
            _targetVolumeScale = 1f;
            _targetCutoffHz = MaxCutoffFrequency;
        }

        private void ApplyDefaultMuffleToSample(int index)
        {
            _samples[index].CalculatedVolumeScale = 0.2f;
            _samples[index].CalculatedCutoffHz = MinCutoffFrequency;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Vector3 start = transform.position;
            if (_cachedListener == null) return;
            Vector3 listenerPos = _cachedListener.transform.position;

            Vector3 direction = listenerPos - start;
            // BOLT OPTIMIZATION: rightNorm is normalized. Cross product of two normalized, perpendicular vectors (dirNorm and rightNorm) is inherently normalized.
            // Avoids calling .normalized on the second cross product and avoids multiplying by SpreadWidth twice.
            Vector3 dirNorm = direction.normalized;
            Vector3 rightNorm = Vector3.Cross(dirNorm, Vector3.up).normalized;
            Vector3 upNorm = Vector3.Cross(dirNorm, rightNorm);
            Vector3 right = rightNorm * SpreadWidth;
            Vector3 up = upNorm * SpreadWidth;

            for (int i = 0; i < _activeSampleCount; i++)
            {
                Vector3 offset = Vector3.zero;
                switch (i)
                {
                    case 1: offset = -right; break;
                    case 2: offset = right; break;
                    case 3: offset = up; break;
                    case 4: offset = -up; break;
                }

                Vector3 startPos = start + offset;
                Vector3 endPos = listenerPos + offset;

                if (_samples[i].ForwardPending || _samples[i].BackwardPending)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(startPos, endPos);
                }
                else
                {
                    float vol = _samples[i].CalculatedVolumeScale;
                    bool didMuffle = vol < 0.99f;
                    Gizmos.color = didMuffle ? Color.red : Color.green;
                    
                    if (didMuffle)
                    {
                        Gizmos.DrawLine(startPos, _samples[i].ForwardHitPoint);
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawLine(endPos, _samples[i].ForwardHitPoint); // Wall thickness visualizer
                    }
                    else
                    {
                        Gizmos.DrawLine(startPos, endPos);
                    }
                }
            }
        }
#endif
    }
}
