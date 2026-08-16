using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using Systems.Core.Utilities;

namespace MatchmakingSystem.Runtime
{
    /// <summary>
    /// Handles Steam Lobby creation, joining, and interfacing with Unity Netcode's FacepunchTransport.
    /// </summary>
    public class SteamLobbyManager : MonoBehaviour
    {
        public static SteamLobbyManager Instance { get; private set; }

        public Lobby? CurrentLobby { get; private set; }

        public event Action<Lobby> OnLobbyCreated;
        public event Action<Lobby> OnLobbyJoined;
        public event Action OnLobbyLeft;
        public event Action<Lobby[]> OnLobbyListFetched;

        [Header("Settings")]
        public int MaxPlayers = 4;
        
        private FacepunchTransport _transport;

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

        private void Start()
        {
            _transport = NetworkManager.Singleton.GetComponent<FacepunchTransport>();
            if (_transport == null)
            {
                Debug.LogError("[SteamLobbyManager] FacepunchTransport missing on NetworkManager!");
            }

            SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberDisconnected += SteamMatchmaking_OnLobbyMemberDisconnected;
            SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
            SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
                SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
                SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
                SteamMatchmaking.OnLobbyMemberDisconnected -= SteamMatchmaking_OnLobbyMemberDisconnected;
                SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
                SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;
            }
        }

        public async void HostLobby()
        {
            if (!SteamClient.IsValid) return;

            Debug.Log("[SteamLobbyManager] Creating lobby...");
            var lobbyCreated = await SteamMatchmaking.CreateLobbyAsync(MaxPlayers);
            if (!lobbyCreated.HasValue)
            {
                Debug.LogError("[SteamLobbyManager] Failed to create lobby.");
                return;
            }
            // The callback SteamMatchmaking_OnLobbyCreated will handle the rest
        }

        public async void FetchLobbies()
        {
            if (!SteamClient.IsValid) return;

            Debug.Log("[SteamLobbyManager] Fetching lobbies...");
            var lobbies = await SteamMatchmaking.LobbyList.WithMaxResults(20).RequestAsync();
            OnLobbyListFetched?.Invoke(lobbies);
        }

        public async void JoinLobby(Lobby lobby)
        {
            if (!SteamClient.IsValid) return;
            Debug.Log($"[SteamLobbyManager] Joining lobby {lobby.Id}...");
            await lobby.Join();
            // The callback SteamMatchmaking_OnLobbyEntered will handle the rest
        }

        public void LeaveLobby()
        {
            if (CurrentLobby.HasValue)
            {
                CurrentLobby.Value.Leave();
                CurrentLobby = null;
                NetworkManager.Singleton.Shutdown();
                OnLobbyLeft?.Invoke();
                Debug.Log("[SteamLobbyManager] Left lobby and shut down NetworkManager.");
            }
        }

        private void SteamMatchmaking_OnLobbyCreated(Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                Debug.LogError($"[SteamLobbyManager] Lobby creation failed: {result}");
                return;
            }

            lobby.SetPublic(); // Or Private if you only want invites
            lobby.SetJoinable(true);
            lobby.SetData("HostName", StringUtilities.SanitizeForRichText(SteamClient.Name));

            CurrentLobby = lobby;
            
            // Start Host
            _transport.targetSteamId = SteamClient.SteamId; // Not strictly necessary for host, but good practice
            NetworkManager.Singleton.StartHost();

            OnLobbyCreated?.Invoke(lobby);
            Debug.Log($"[SteamLobbyManager] Lobby created & Host started. Lobby ID: {lobby.Id}");
        }

        private void SteamMatchmaking_OnLobbyEntered(Lobby lobby)
        {
            if (NetworkManager.Singleton.IsHost) return; // Host already handled this in OnLobbyCreated

            CurrentLobby = lobby;
            
            // Find host steam ID from lobby owner
            _transport.targetSteamId = lobby.Owner.Id;
            
            NetworkManager.Singleton.StartClient();

            OnLobbyJoined?.Invoke(lobby);
            Debug.Log($"[SteamLobbyManager] Joined lobby & Client started. Host ID: {lobby.Owner.Id}");
        }

        private void SteamMatchmaking_OnLobbyMemberJoined(Lobby lobby, Friend friend)
        {
            Debug.Log($"[SteamLobbyManager] {friend.Name} joined the lobby.");
        }

        private void SteamMatchmaking_OnLobbyMemberDisconnected(Lobby lobby, Friend friend)
        {
            Debug.Log($"[SteamLobbyManager] {friend.Name} disconnected from the lobby.");
        }

        private void SteamMatchmaking_OnLobbyMemberLeave(Lobby lobby, Friend friend)
        {
            Debug.Log($"[SteamLobbyManager] {friend.Name} left the lobby.");
        }

        private async void SteamFriends_OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
        {
            // Triggered when accepting an invite via Steam Overlay
            await lobby.Join();
        }
    }
}
