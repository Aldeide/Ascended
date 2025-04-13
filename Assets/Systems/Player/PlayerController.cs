using FishNet.Connection;
using FishNet.Object;
using Systems.AbilitySystem.Components;
using Systems.Camera;
using Systems.Development;
using Systems.Interface;
using UnityEngine;

namespace Systems.Player
{
    public class PlayerController : NetworkBehaviour
    {
        private InterfaceController _interfaceController;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;
            SetupCamera();
            SetupAsc();
            SetupInterface();
            SetupDebug();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            SetupAsc();
            SetupDebug();
        }

        private void SetupCamera()
        {
            GameObject.Find("CameraTarget").GetComponent<CameraTargetController>().SetTarget(this.transform);
        }

        private void SetupAsc()
        {
            GetComponent<AbilitySystemComponent>().Initialise();
        }

        private void SetupInterface()
        {
            _interfaceController = GameObject.Find("Interface").GetComponent<InterfaceController>();
            _interfaceController.Initialise(GetComponent<AbilitySystemComponent>());
        }

        private void SetupDebug()
        {
            var debug = GameObject.Find("Debug/Text").GetComponent<DebugComponent>();
            debug.player = this.gameObject;
            debug.SetOwner(GetComponent<AbilitySystemComponent>());
        }
    }
}