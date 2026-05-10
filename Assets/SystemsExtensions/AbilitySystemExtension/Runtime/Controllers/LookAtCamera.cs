using UnityEngine;

namespace Systems.Controllers
{
    [ExecuteAlways]
    public class LookAtCamera : MonoBehaviour
    {
        public RectTransform RectangleTransform;
        public GameObject Entity;
        private UnityEngine.Camera _camera;
        public Vector3 Offset;
        public void Start()
        {
            _camera = UnityEngine.Camera.main;
            Offset = new Vector3(0, 2.2f, 0);
        }

        public void LateUpdate()
        {
            if(!RectangleTransform || !Entity) return;

            // Position the unitframe.
            Vector3 screenPosition = _camera.WorldToScreenPoint(Entity.transform.position + Offset);
            
            // If the entity is behind the camera (z < 0), hide it by moving off-screen.
            if (screenPosition.z < 0)
            {
                RectangleTransform.anchoredPosition = new Vector2(-10000, -10000);
                return;
            }
            
            // In Screen Space Overlay, we don't need to rotate (it's always screen-facing).
            // But we do need to handle the offset from the screen center if anchors are at (0.5, 0.5).
            // Here we assume anchors are at (0,0) or we convert to local space.
            
            // To make it easy, we'll just set the anchoredPosition.
            // If the canvas is 1:1 with pixels (Overlay usually is), we can just use screenPosition.
            RectangleTransform.position = screenPosition;
        }
    }
}