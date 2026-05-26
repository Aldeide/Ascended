using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Systems.Audio
{
    /// <summary>
    /// Callback interface to receive raycast results without GC allocation.
    /// </summary>
    public interface IAudioRaycastReceiver
    {
        void OnRaycastComplete(RaycastHit hit, int requestId);
    }

    [DefaultExecutionOrder(-100)] // Run before other audio components queue their raycasts
    public class AudioRaycastManager : MonoBehaviour
    {
        private static AudioRaycastManager _instance;
        public static AudioRaycastManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<AudioRaycastManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AudioRaycastManager");
                        _instance = go.AddComponent<AudioRaycastManager>();
                    }
                }
                return _instance;
            }
        }

        [Header("Configuration")]
        [Tooltip("The default physics layers that acoustic rays will collide with.")]
        public LayerMask DefaultLayerMask = -1; // Collide with everything by default

        [Tooltip("Maximum number of acoustic raycasts allowed per frame.")]
        [Range(32, 2048)]
        public int MaxRaycastsPerFrame = 512;

        private struct QueuedRaycast
        {
            public Vector3 Origin;
            public Vector3 Direction;
            public float MaxDistance;
            public IAudioRaycastReceiver Receiver;
            public int RequestId;
            public int LayerMask;
        }

        // Double buffering/queued lists for requests
        private QueuedRaycast[] _queuedRequests;
        private QueuedRaycast[] _executingRequests;
        private int _queuedCount;

        // Native arrays for physics jobs
        private NativeArray<RaycastCommand> _commands;
        private NativeArray<RaycastHit> _results;
        private JobHandle _jobHandle;
        private bool _jobScheduled;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Pre-allocate arrays to avoid runtime GC allocation
            _queuedRequests = new QueuedRaycast[MaxRaycastsPerFrame];
            _executingRequests = new QueuedRaycast[MaxRaycastsPerFrame];
            _commands = new NativeArray<RaycastCommand>(MaxRaycastsPerFrame, Allocator.Persistent);
            _results = new NativeArray<RaycastHit>(MaxRaycastsPerFrame, Allocator.Persistent);
        }

        private void OnDestroy()
        {
            if (_jobScheduled)
            {
                _jobHandle.Complete();
            }

            if (_commands.IsCreated) _commands.Dispose();
            if (_results.IsCreated) _results.Dispose();
        }

        /// <summary>
        /// Queues a raycast request to be executed on worker threads this frame.
        /// </summary>
        /// <param name="origin">Start point of the ray.</param>
        /// <param name="direction">Direction of the ray. MUST be normalized.</param>
        /// <param name="maxDistance">Maximum distance of the ray.</param>
        /// <param name="receiver">Interface receiver to get the callback.</param>
        /// <param name="requestId">Custom ID to identify the raycast on callback.</param>
        /// <param name="layerMask">Optional layer mask override. If -1, uses DefaultLayerMask.</param>
        public void QueueRaycast(Vector3 origin, Vector3 direction, float maxDistance, IAudioRaycastReceiver receiver, int requestId, int layerMask = -1)
        {
            if (_queuedCount >= MaxRaycastsPerFrame)
            {
                Debug.LogWarning($"[AudioRaycastManager] Exceeded MaxRaycastsPerFrame limit ({MaxRaycastsPerFrame}). Dropping request.");
                return;
            }

            if (receiver == null) return;

            _queuedRequests[_queuedCount] = new QueuedRaycast
            {
                Origin = origin,
                Direction = direction,
                MaxDistance = maxDistance,
                Receiver = receiver,
                RequestId = requestId,
                LayerMask = layerMask == -1 ? (int)DefaultLayerMask : layerMask
            };
            _queuedCount++;
        }

        private void LateUpdate()
        {
            if (_queuedCount == 0) return;

            // Copy queued requests to the executing requests array and clear the main queue
            int countToExecute = _queuedCount;
            System.Array.Copy(_queuedRequests, _executingRequests, countToExecute);
            _queuedCount = 0;

            // 1. Build the commands array
            for (int i = 0; i < countToExecute; i++)
            {
                var req = _executingRequests[i];
                // In Unity 6, RaycastCommand constructor: (from, direction, distance, layerMask, maxHits)
                _commands[i] = new RaycastCommand(
                    req.Origin,
                    req.Direction, // Direction is expected to be normalized by caller to save Sqrt costs
                    req.MaxDistance,
                    req.LayerMask,
                    1 // Only need the first hit
                );
            }

            // 2. Schedule the batched raycast job
            _jobHandle = RaycastCommand.ScheduleBatch(_commands, _results, 8);
            _jobScheduled = true;

            // 3. Force complete the job at the end of LateUpdate so scripts can receive results immediately
            // This gives the worker threads maximum time between Update and LateUpdate to complete the physics queries
            _jobHandle.Complete();
            _jobScheduled = false;

            // 4. Distribute results allocation-free
            for (int i = 0; i < countToExecute; i++)
            {
                var req = _executingRequests[i];
                var hit = _results[i];
                req.Receiver.OnRaycastComplete(hit, req.RequestId);
            }
        }
    }
}
