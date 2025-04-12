using FishNet.Connection;
using FishNet.Object;
using Systems.Camera;
using Systems.Development;
using Systems.Interface;
using UnityEngine;

namespace Systems.Player
{
    public class PlayerController : NetworkBehaviour
    {
        private InterfaceController _interfaceController;
        
        public void Start()
        {
            Debug.Log("OwnershipClient");
            if (!Owner.IsLocalClient) return;
            Debug.Log("OwnershipClient2");
            _interfaceController = GameObject.Find("Interface").GetComponent<InterfaceController>();
            _interfaceController.Initialise();
            GameObject.Find("CameraTarget").GetComponent<CameraTargetController>().SetTarget(this.transform);
            GameObject.Find("Debug/Text").GetComponent<DebugComponent>().player = this.gameObject;
        }
    }
}