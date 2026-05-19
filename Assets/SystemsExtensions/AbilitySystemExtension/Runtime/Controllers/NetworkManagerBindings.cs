using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SystemsExtensions.AbilitySystemExtension.Runtime.Controllers
{
    public class NetworkManagerBindings : MonoBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;
        private void Awake()
        {
            _networkManager = GetComponent<NetworkManager>();
        }
        
        
        public void OnSpawnHost(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed) return;
            _networkManager.StartHost();
        }

        public void OnSpawnClient(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed) return;
            _networkManager.StartClient();
        }

        public void OnSpawnServer(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed) return;
            _networkManager.StartServer();
        }
    }
}