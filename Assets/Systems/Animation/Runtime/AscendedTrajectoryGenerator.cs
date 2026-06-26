using UnityEngine;
using Unity.Mathematics;

namespace Ascended.Systems.Animation.Runtime
{
    /// <summary>
    /// Predicts the future trajectory of the character using either input-smoothing 
    /// or physics-based velocity projection.
    /// </summary>
    public class AscendedTrajectoryGenerator : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("If true, trajectory is predicted using physical velocity. If false, it uses smoothed player input.")]
        public bool UsePhysicsDrivenTrajectory = false;
        
        [Header("Input-Driven Settings")]
        [SerializeField] private float _inputSmoothing = 10f;
        [SerializeField] private float _maxSpeed = 5f;

        [Header("Physics-Driven Settings")]
        [Tooltip("Assign the Rigidbody if using Physics-driven trajectory.")]
        [SerializeField] private Rigidbody _targetRigidbody; 
        
        private Vector2 _currentInput;
        private Vector2 _smoothInput;
        
        // Expose the 3 future trajectory points (e.g. at 0.33s, 0.66s, 1.0s)
        public TrajectoryPointData Trajectory0 { get; private set; }
        public TrajectoryPointData Trajectory1 { get; private set; }
        public TrajectoryPointData Trajectory2 { get; private set; }

        /// <summary>
        /// Updates the current raw movement input.
        /// </summary>
        public void SetInput(Vector2 input)
        {
            _currentInput = Vector2.ClampMagnitude(input, 1f);
        }

        private void Update()
        {
            if (UsePhysicsDrivenTrajectory)
            {
                UpdatePhysicsDrivenTrajectory(Time.deltaTime);
            }
            else
            {
                UpdateInputDrivenTrajectory(Time.deltaTime);
            }
        }

        private void UpdateInputDrivenTrajectory(float deltaTime)
        {
            // Smooth input for organic movement curves
            _smoothInput = Vector2.Lerp(_smoothInput, _currentInput, _inputSmoothing * deltaTime);
            Debug.Log($"[MM TrajectoryGen] currentInput={_currentInput}, smoothInput={_smoothInput}, maxSpeed={_maxSpeed}");
            
            // Convert input to world space relative to the camera's yaw
            Vector3 inputWorld = Vector3.zero;
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                Vector3 camForward = mainCam.transform.forward;
                camForward.y = 0f;
                camForward.Normalize();
                Vector3 camRight = mainCam.transform.right;
                camRight.y = 0f;
                camRight.Normalize();
                inputWorld = camForward * _smoothInput.y + camRight * _smoothInput.x;
            }
            else
            {
                Vector3 forward = transform.forward;
                forward.y = 0f;
                forward.Normalize();
                Vector3 right = transform.right;
                right.y = 0f;
                right.Normalize();
                inputWorld = forward * _smoothInput.y + right * _smoothInput.x;
            }
            
            Vector3 predictedPos = transform.position;
            float predictedFacing = transform.rotation.eulerAngles.y;
            
            float timeStep = 0.33f;
            Vector3 moveDelta = inputWorld * (_maxSpeed * timeStep);

            // Update facing direction if we are moving
            if (moveDelta.sqrMagnitude > 0.0001f)
            {
                predictedFacing = Vector3.SignedAngle(Vector3.forward, moveDelta.normalized, Vector3.up);
            }

            Trajectory0 = new TrajectoryPointData {
                Position = predictedPos + moveDelta * 1,
                Velocity = inputWorld * _maxSpeed,
                FacingAngle = predictedFacing
            };
            
            Trajectory1 = new TrajectoryPointData {
                Position = predictedPos + moveDelta * 2,
                Velocity = inputWorld * _maxSpeed,
                FacingAngle = predictedFacing
            };

            Trajectory2 = new TrajectoryPointData {
                Position = predictedPos + moveDelta * 3,
                Velocity = inputWorld * _maxSpeed,
                FacingAngle = predictedFacing
            };
        }

        private void UpdatePhysicsDrivenTrajectory(float deltaTime)
        {
            // Use actual physics velocity
            Vector3 currentVel = _targetRigidbody != null ? _targetRigidbody.linearVelocity : Vector3.zero;
            currentVel.y = 0; // Flatten trajectory prediction to horizontal plane
            
            Vector3 predictedPos = transform.position;
            float predictedFacing = transform.rotation.eulerAngles.y;

            if (currentVel.sqrMagnitude > 0.0001f)
            {
                predictedFacing = Vector3.SignedAngle(Vector3.forward, currentVel.normalized, Vector3.up);
            }
            
            float timeStep = 0.33f;

            Trajectory0 = new TrajectoryPointData {
                Position = predictedPos + currentVel * (timeStep * 1),
                Velocity = currentVel,
                FacingAngle = predictedFacing
            };
            
            Trajectory1 = new TrajectoryPointData {
                Position = predictedPos + currentVel * (timeStep * 2),
                Velocity = currentVel,
                FacingAngle = predictedFacing
            };

            Trajectory2 = new TrajectoryPointData {
                Position = predictedPos + currentVel * (timeStep * 3),
                Velocity = currentVel,
                FacingAngle = predictedFacing
            };
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.cyan;
            
            DrawTrajectoryPoint(Trajectory0);
            DrawTrajectoryPoint(Trajectory1);
            DrawTrajectoryPoint(Trajectory2);
        }

        private void DrawTrajectoryPoint(TrajectoryPointData tp)
        {
            Gizmos.DrawSphere(tp.Position, 0.1f);
            Vector3 facingDir = Quaternion.Euler(0, tp.FacingAngle, 0) * Vector3.forward;
            Gizmos.DrawRay(tp.Position, facingDir * 0.4f);
        }
#endif
    }
}
