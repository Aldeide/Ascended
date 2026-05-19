using Unity.Netcode;
using UnityEngine;

namespace MatchmakingSystem.Runtime
{
    /// <summary>
    /// Place this script on an empty GameObject in your Gameplay Scene (e.g., SampleScene).
    /// When the scene finishes loading for a client, the server will automatically spawn
    /// a player avatar for them.
    /// </summary>
    public class PlayerSpawner : NetworkBehaviour
    {
        [Header("Settings")]
        [Tooltip("The Player Prefab to spawn. Make sure this is also in your NetworkManager's NetworkPrefabs list!")]
        public GameObject PlayerPrefab;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // Hook into the scene loading event. 
                // This is the safest place to spawn because it ensures the client is ready.
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer && NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
            }
        }

        private void OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
        {
            if (!IsServer) return;

            foreach (var clientId in clientsCompleted)
            {
                SpawnPlayerForClient(clientId);
            }
        }

        private void SpawnPlayerForClient(ulong clientId)
        {
            // Ensure we don't spawn multiple players for the same client
            var networkClient = NetworkManager.Singleton.ConnectedClients[clientId];
            if (networkClient.PlayerObject != null) return;

            var playerInstance = Instantiate(PlayerPrefab, new Vector3(0, 10, 0), Quaternion.identity);
            var networkObject = playerInstance.GetComponent<NetworkObject>();
            
            networkObject.SpawnAsPlayerObject(clientId, true);
        }
    }
}
