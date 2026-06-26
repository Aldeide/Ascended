using Unity.Netcode;
using UnityEngine;

namespace Ascended.Systems.Animation.Runtime
{
    /// <summary>
    /// Interface for components that provide movement input to the motion matching system.
    /// Helps decouple movement logic from animation systems.
    /// </summary>
    public interface IMotionInputProvider
    {
        Vector2 GetMovementInput();
        bool CanMove();
    }

    /// <summary>
    /// Network wrapper for the motion matching animation system.
    /// Replicates the player's input over the network so that proxy clients can 
    /// evaluate the trajectory and perform motion matching locally.
    /// </summary>
    [RequireComponent(typeof(AscendedTrajectoryGenerator))]
    public class AscendedMotionController : NetworkBehaviour
    {
        private AscendedTrajectoryGenerator _trajectoryGenerator;
        private IMotionInputProvider _inputProvider;

        // Replicated movement input from the owner client
        private readonly NetworkVariable<Vector2> _replicatedInput = new NetworkVariable<Vector2>(
            default,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        private Vector2 _lastSentInput;

        private void Awake()
        {
            _trajectoryGenerator = GetComponent<AscendedTrajectoryGenerator>();
            
            // Robust search in hierarchy
            _inputProvider = GetComponent<IMotionInputProvider>();
            if (_inputProvider == null) _inputProvider = GetComponentInParent<IMotionInputProvider>();
            if (_inputProvider == null) _inputProvider = GetComponentInChildren<IMotionInputProvider>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _lastSentInput = Vector2.zero;
            }
        }

        private void Update()
        {
            // If offline/not spawned, or if we are the network owner, gather input
            if (!IsSpawned || IsOwner)
            {
                Vector2 currentInput = Vector2.zero;
                
                if (_inputProvider != null)
                {
                    if (_inputProvider.CanMove())
                    {
                        currentInput = _inputProvider.GetMovementInput();
                    }
                    else
                    {
                        if (Time.frameCount % 60 == 0)
                        {
                            Debug.LogWarning("[MM Controller] inputProvider.CanMove() is FALSE!");
                        }
                    }
                }
                else
                {
                    if (Time.frameCount % 60 == 0)
                    {
                        Debug.LogWarning("[MM Controller] inputProvider is NULL!");
                    }
                }

                // If offline (not spawned on network), apply input directly and return
                if (!IsSpawned)
                {
                    if (_trajectoryGenerator != null)
                    {
                        _trajectoryGenerator.SetInput(currentInput);
                    }
                    return;
                }

                // If online, replicate the owner client's input via Netcode
                if (Vector2.SqrMagnitude(_lastSentInput - currentInput) > 0.0001f)
                {
                    _lastSentInput = currentInput;
                    
                    if (IsServer)
                    {
                        _replicatedInput.Value = currentInput;
                    }
                    else
                    {
                        UpdateInputServerRpc(currentInput);
                    }
                }
            }

            // Feed the replicated input value into the local trajectory generator (all clients + server)
            if (_trajectoryGenerator != null)
            {
                _trajectoryGenerator.SetInput(_replicatedInput.Value);
            }
        }

        /// <summary>
        /// Sends the local owner's input vector to the server for replication.
        /// </summary>
        [ServerRpc(RequireOwnership = true)]
        private void UpdateInputServerRpc(Vector2 input)
        {
            _replicatedInput.Value = input;
        }
    }
}
