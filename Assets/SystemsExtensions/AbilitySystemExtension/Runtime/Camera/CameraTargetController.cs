using UnityEngine;

namespace Systems.Camera
{
    public class CameraTargetController : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Vector3 _offset = new Vector3(0, 1.5f, 0);
        
        private void LateUpdate()
        {
            if (!_player) return;
            transform.position = Vector3.Lerp(transform.position, _player.transform.position + _offset, Time.deltaTime * 20f);
        }

        public void SetTarget(Transform transform)
        {
            _player = transform;
        }
    }
}