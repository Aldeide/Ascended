using Unity.Netcode;
using UnityEngine;

namespace Systems.Controllers
{
    public class LifetimeController : NetworkBehaviour
    {
        public float maxLifetime;
        private float _current = 0;

        public void Update()
        {
            if (!IsServer) return;
            _current += Time.deltaTime;
            if (_current >= maxLifetime)
            {
                Destroy(this.gameObject);
            }
        }
    }
}