using FishNet;
using UnityEngine;

namespace Systems.Networking
{
    public class ClientServerManager : MonoBehaviour
    {
        [SerializeField]
        private bool isServer;
        [SerializeField]
        private bool isClient;

        private void Awake()
        {
            if (isServer) InstanceFinder.ServerManager.StartConnection();
            if (isClient) InstanceFinder.ClientManager.StartConnection();
            //InstanceFinder.ClientManager.StartConnection();
        }
    }
}