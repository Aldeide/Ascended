using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace Systems.Targeting
{
    public class PlayerTargetController : MonoBehaviour
    {
        public GameObject target;
        public LayerMask layerMask;
        public void Start()
        {
            
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed) return;
            UpdateTarget();
        }

        public void UpdateTarget()
        {
            Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f); // center of the screen
            float rayLength = 500f;
            Ray ray = UnityEngine.Camera.main.ViewportPointToRay(rayOrigin);
            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

            RaycastHit hit;
            
            
            if (Physics.Raycast(ray, out hit, rayLength, layerMask, QueryTriggerInteraction.Ignore))
            {
                target.transform.position = hit.point;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(target.transform.position, new Vector3(0.1f, 0.1f, 0.1f));
        }
    }
}