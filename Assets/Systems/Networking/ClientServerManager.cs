using FishNet;
using UnityEngine;

namespace Systems.Networking
{
    public class ClientServerManager : MonoBehaviour
    {
        [SerializeField]
        private bool isServer;

        private void Awake()
        {
            if (isServer) InstanceFinder.ServerManager.StartConnection();
            else InstanceFinder.ClientManager.StartConnection();
            //InstanceFinder.ClientManager.StartConnection();
        }
    }
}