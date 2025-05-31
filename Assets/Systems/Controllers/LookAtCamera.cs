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

        public void Update()
        {
            if(!RectangleTransform || !Entity) return;

            // Position the unitframe.
            Vector2 position = _camera.WorldToScreenPoint(Entity.transform.position + Offset);
            RectangleTransform.anchoredPosition = new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            
            // Billboard the unitframe.
            transform.LookAt(RectangleTransform.position - _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
            //transform.forward = _camera.transform.forward * _camera.transform.rotation;
        }
    }
}