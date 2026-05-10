using System;
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
            this._networkManager = GetComponent<NetworkManager>();
        }
        
        
        public void OnSpawnHost(InputAction.CallbackContext context)
        {
            Debug.Log("OnSpawnHost");
            if (context.phase == InputActionPhase.Performed)
            {
                Debug.Log("OnSpawnHostCalled");
                _networkManager.StartHost();
            }
        }

        public void OnSpawnClient(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                _networkManager.StartClient();
            }
        }

        public void OnSpawnServer(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                _networkManager.StartServer();
            }
        }
    }
}