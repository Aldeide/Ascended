using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using Systems.Camera;
using Unity.Netcode;
using UnityEngine;

namespace Systems.Controllers
{
    public class PlayerController : NetworkBehaviour
    {
        private InterfaceController _interfaceController;

        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (IsSpawned && IsLocalPlayer)
            {
                this.transform.position = new Vector3(0, 10, 0);
                SetupCamera();
                SetupInterface();
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsLocalPlayer)
            {
                SetupCamera();
            }
            SetupAsc();
            if (IsLocalPlayer)
            {
                SetupInterface();
            }
        }

        private void SetupCamera()
        {
            var cameraTarget = GameObject.Find("CameraTarget");
            if (cameraTarget != null)
            {
                cameraTarget.GetComponent<CameraTargetController>()?.SetTarget(this.transform);
            }
        }

        private void SetupAsc()
        {
            GetComponent<AbilitySystemComponent>().Initialise();
        }

        private void SetupInterface()
        {
            var interfaceObj = GameObject.Find("Interface");
            if (interfaceObj != null)
            {
                _interfaceController = interfaceObj.GetComponent<InterfaceController>();
                _interfaceController?.Initialise(GetComponent<AbilitySystemComponent>().AbilitySystem as AbilitySystemManager);
            }
        }
    }
}