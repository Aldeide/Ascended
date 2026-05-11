using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace MatchmakingSystem.Runtime
{
    /// <summary>
    /// Represents the network state of a player in the lobby.
    /// </summary>
    public struct LobbyPlayerState : INetworkSerializable, IEquatable<LobbyPlayerState>
    {
        public ulong ClientId;
        public FixedString64Bytes PlayerName;
        public bool IsReady;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref IsReady);
        }

        public bool Equals(LobbyPlayerState other)
        {
            return ClientId == other.ClientId &&
                   PlayerName.Equals(other.PlayerName) &&
                   IsReady == other.IsReady;
        }
    }

    /// <summary>
    /// NetworkBehaviour responsible for tracking and synchronizing all lobby players' states (like "Ready").
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkLobbyState : NetworkBehaviour
    {
        public static NetworkLobbyState Instance { get; private set; }

        public NetworkList<LobbyPlayerState> LobbyPlayers = new NetworkList<LobbyPlayerState>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public override void OnNetworkSpawn()
        {
            Debug.Log($"[NetworkLobbyState] OnNetworkSpawn. IsServer: {IsServer}, IsClient: {IsClient}");

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
                
                // Add host immediately
                AddPlayerToList(NetworkManager.Singleton.LocalClientId);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                if (NetworkManager.Singleton != null)
                {
                    NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                    NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
                }
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            AddPlayerToList(clientId);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            RemovePlayerFromList(clientId);
        }

        private void AddPlayerToList(ulong clientId)
        {
            foreach (var p in LobbyPlayers)
            {
                if (p.ClientId == clientId) return; // Already exists
            }

            // By default, just use "Player X". A real game would query SteamFriends.GetFriendPersonaName.
            var playerName = $"Player {clientId}"; 

            LobbyPlayers.Add(new LobbyPlayerState
            {
                ClientId = clientId,
                PlayerName = new FixedString64Bytes(playerName),
                IsReady = false
            });

            Debug.Log($"[NetworkLobbyState] Added player {clientId} to list. Total players: {LobbyPlayers.Count}");
        }

        private void RemovePlayerFromList(ulong clientId)
        {
            for (int i = 0; i < LobbyPlayers.Count; i++)
            {
                if (LobbyPlayers[i].ClientId == clientId)
                {
                    LobbyPlayers.RemoveAt(i);
                    break;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ToggleReadyServerRpc(ServerRpcParams rpcParams = default)
        {
            ulong senderId = rpcParams.Receive.SenderClientId;
            for (int i = 0; i < LobbyPlayers.Count; i++)
            {
                if (LobbyPlayers[i].ClientId == senderId)
                {
                    var playerState = LobbyPlayers[i];
                    playerState.IsReady = !playerState.IsReady;
                    LobbyPlayers[i] = playerState;
                    break;
                }
            }
        }

        public bool AreAllPlayersReady()
        {
            if (LobbyPlayers.Count == 0) return false;

            foreach (var player in LobbyPlayers)
            {
                if (!player.IsReady)
                    return false;
            }
            return true;
        }

        public void StartGame(string sceneName)
        {
            if (IsServer && AreAllPlayersReady())
            {
                NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            else
            {
                Debug.LogWarning("[NetworkLobbyState] Cannot start game: Not server or not all players ready.");
            }
        }
    }
}
