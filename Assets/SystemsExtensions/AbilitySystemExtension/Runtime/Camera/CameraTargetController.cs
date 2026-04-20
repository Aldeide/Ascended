using UnityEngine;

namespace Systems.Camera
{
    public class CameraTargetController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
        
        private void Update()
        {
            if (player == null) return;
            this.transform.position = player.transform.position + offset;
        }

        public void SetTarget(Transform transform)
        {
            player = transform;
        }
    }
}