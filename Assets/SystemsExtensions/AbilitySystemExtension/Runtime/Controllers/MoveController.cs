using UnityEngine;

namespace Systems.Controllers
{
    public class MoveController : MonoBehaviour
    {
        public Vector3 TargetPosition = new(0,0,0);
        public float Speed = 1.0f;
        
        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, Time.deltaTime * Speed);
        }
    }
}