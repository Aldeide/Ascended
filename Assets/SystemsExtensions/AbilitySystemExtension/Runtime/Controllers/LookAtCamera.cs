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
            
            RectangleTransform.anchoredPosition = new Vector2(Mathf.RoundToInt(screenPosition.x), Mathf.RoundToInt(screenPosition.y));
            
            // Billboard the unitframe.
            transform.LookAt(RectangleTransform.position - _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
            //transform.forward = _camera.transform.forward * _camera.transform.rotation;
        }
    }
}