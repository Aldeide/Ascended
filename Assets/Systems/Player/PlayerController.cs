using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using AbilitySystemExtension.Scripts;
using Systems.Camera;
using Systems.Interface;
using Unity.Netcode;
using UnityEngine;

namespace Systems.Player
{
    public class PlayerController : NetworkBehaviour
    {
        private InterfaceController _interfaceController;

        public override void OnNetworkSpawn()
        {
            if (!HasAuthority) return;
            base.OnNetworkSpawn();
            SetupCamera();
            SetupAsc();
            SetupInterface();
            SetupDebug();
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

        private void SetupDebug()
        {
            var debug = GameObject.Find("Debug/Text").GetComponent<AbilitySystemDebugComponent>();
            debug.Initialise(GetComponent<AbilitySystemComponent>().AbilitySystem as AbilitySystemManager);
        }
    }
}