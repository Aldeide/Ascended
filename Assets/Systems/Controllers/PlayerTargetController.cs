using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Systems.Controllers
{
    public class PlayerTargetController : NetworkBehaviour
    {
        [FormerlySerializedAs("target")] public GameObject Target;
        [FormerlySerializedAs("layerMask")] public LayerMask LayerMask;
        
        private UnityEngine.Camera _camera;
        
        public void Start()
        {
            _camera = UnityEngine.Camera.main;
        }

        public Vector3 GetTargetPosition()
        {
            return Target.transform.position;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (!IsLocalPlayer) return;
            if (context.phase != InputActionPhase.Performed) return;
            UpdateTarget();
        }

        private void UpdateTarget()
        {
            var rayOrigin = new Vector3(0.5f, 0.5f, 0f); // center of the screen
            const float rayLength = 500f;
            var ray = _camera.ViewportPointToRay(rayOrigin);
            
            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);
            
            if (Physics.Raycast(ray, out var hit, rayLength, LayerMask, QueryTriggerInteraction.Ignore))
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