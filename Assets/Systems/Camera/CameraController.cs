using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Systems.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private float rotationSpeed = 1.0f;
        [SerializeField] private float topAngleClamp = 40f;
        [SerializeField] private float bottomAngleClamp = -40f;
        private Vector2 mouseDelta = new();
        private float cinemachineTargetPitch = 0f;
        private float cinemachineTargetYaw = 0f;

        public GameObject defaultVirtualCamera;
        public GameObject aimVirtualCamera;
        
        public void OnLook(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed) return;
            //mouseDelta = context.ReadValue<Vector2>();
            UpdateCamera(context.ReadValue<Vector2>());
        }

        private void UpdateCamera(Vector2 mouseInput)
        {
            float mouseX = mouseInput.x;
            float mouseY = mouseInput.y;
            
            cinemachineTargetPitch =
                UpdateRotation(cinemachineTargetPitch, mouseY, bottomAngleClamp, topAngleClamp, true);
            cinemachineTargetYaw = UpdateRotation(cinemachineTargetYaw, mouseX, float.MinValue, float.MaxValue, false);
            followTarget.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, followTarget.eulerAngles.z);
        }

        private float UpdateRotation(float currentRotation, float input, float min, float max, bool isXAxis)
        {
            currentRotation += isXAxis ? -input : input;
            return Mathf.Clamp(currentRotation, min, max);
        }
    }
}