using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Item.Scripts.UI;

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

        private CameraTargetController _targetController;
        private InventoryUIController _inventoryUI;

        private InventoryUIController GetInventoryUI()
        {
            if (_inventoryUI == null && followTarget != null)
            {
                if (_targetController == null)
                {
                    _targetController = followTarget.GetComponent<CameraTargetController>();
                }
                if (_targetController != null && _targetController.Player != null)
                {
                    _inventoryUI = _targetController.Player.GetComponentInChildren<InventoryUIController>();
                }
            }
            return _inventoryUI;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed) return;
            mouseDelta = context.ReadValue<Vector2>();
        }

        private void Update()
        {
            var inv = GetInventoryUI();
            if (inv != null && inv.IsMenuOpen)
            {
                mouseDelta = Vector2.zero;
                return;
            }

            if (mouseDelta.sqrMagnitude > 0.01f)
            {
                UpdateCamera(mouseDelta);
                mouseDelta = Vector2.zero; // Clear after use or keep if using continuous input
            }
        }

        private void UpdateCamera(Vector2 mouseInput)
        {
            float mouseX = mouseInput.x;
            float mouseY = mouseInput.y;

            cinemachineTargetPitch =
                UpdateRotation(cinemachineTargetPitch, mouseY, bottomAngleClamp, topAngleClamp, true);
            cinemachineTargetYaw = UpdateRotation(cinemachineTargetYaw, mouseX, float.MinValue, float.MaxValue, false);
            
            followTarget.rotation =
                Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0f);
        }

        private float UpdateRotation(float currentRotation, float input, float min, float max, bool isXAxis)
        {
            currentRotation += isXAxis ? -input : input;
            return Mathf.Clamp(currentRotation, min, max);
        }
    }
}