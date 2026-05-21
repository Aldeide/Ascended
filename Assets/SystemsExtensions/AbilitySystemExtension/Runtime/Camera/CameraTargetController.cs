using UnityEngine;
using Item.Scripts.UI;

namespace Systems.Camera
{
    public class CameraTargetController : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Vector3 _offset = new Vector3(0, 1.5f, 0);

        public Transform Player => _player;

        private UnityEngine.Camera _mainCam;
        private InventoryUIController _inventoryUI;

        private UnityEngine.Camera GetMainCam()
        {
            if (_mainCam == null)
            {
                _mainCam = UnityEngine.Camera.main;
            }
            return _mainCam;
        }

        private InventoryUIController GetInventoryUI()
        {
            if (_inventoryUI == null && _player != null)
            {
                _inventoryUI = _player.GetComponentInChildren<InventoryUIController>();
            }
            return _inventoryUI;
        }
        
        private void LateUpdate()
        {
            if (!_player) return;

            Vector3 targetPos = _player.transform.position + _offset;

            var inv = GetInventoryUI();
            if (inv != null && inv.IsMenuOpen)
            {
                var cam = GetMainCam();
                if (cam != null)
                {
                    // Shift the camera target to the right (camera's right) so the player frames on the left of the screen
                    targetPos += cam.transform.right * 1.2f;
                }
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 20f);
        }

        public void SetTarget(Transform transform)
        {
            _player = transform;
        }
    }
}