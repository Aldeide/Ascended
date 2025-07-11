using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Controllers
{
    public class PlayerTargetController : NetworkBehaviour
    {
        [Header("Configuration")]
        public GameObject Target;
        public LayerMask LayerMask;
        
        [Header("Raycast Settings")]
        private const float MaxRaycastDistance = 500f;
        private static readonly Vector3 ScreenCenter = new Vector3(0.5f, 0.5f, 0f);


        [Header("Debug")]
        private const float GizmoCubeSize = 0.1f;
        
        private UnityEngine.Camera _camera;
        
        public void Start()
        {
            _camera = UnityEngine.Camera.main;
        }

        public Vector3 GetTargetPosition()
        {
            return Target ? Target.transform.position : Vector3.zero;
        }

        public void Update()
        {
            
            if (!IsLocalPlayer) return;
            UpdateTargetPosition();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            // Since the rotation of the player isn't instantaneous, the target tends to move with the player even when
            // no input is given. This is a problem when the player is looking at the target.
            // TODO: Find a way to fix this so when don't have to update the target each frame.
            /*
            if (!IsLocalPlayer || context.phase != InputActionPhase.Performed) 
                return;
            UpdateTargetPosition();
            */
        }

        private void UpdateTargetPosition()
        {
            if (!_camera || !Target) return;

            var ray = _camera.ViewportPointToRay(ScreenCenter);
            
            Debug.DrawRay(ray.origin, ray.direction * MaxRaycastDistance, Color.red);
            
            if (Physics.Raycast(ray, out var hit, MaxRaycastDistance, LayerMask, QueryTriggerInteraction.Ignore))
            {
                Target.transform.position = hit.point;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(Target.transform.position, new Vector3(0.1f, 0.1f, 0.1f));
        }
    }
}