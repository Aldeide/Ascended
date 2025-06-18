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
            GameObject.Find("CameraTarget").GetComponent<CameraTargetController>().SetTarget(this.transform);
        }

        private void SetupAsc()
        {
            GetComponent<AbilitySystemComponent>().Initialise();
            GetComponent<AbilitySystemComponent>().AbilitySystem.AbilityManager.TryActivateAbility("EnergyRegenAbility");
        }

        private void SetupInterface()
        {
            _interfaceController = GameObject.Find("Interface").GetComponent<InterfaceController>();
            _interfaceController.Initialise(GetComponent<AbilitySystemComponent>().AbilitySystem as AbilitySystemManager);
        }
    }
}