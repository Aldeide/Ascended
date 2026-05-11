using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using MatchmakingSystem.Runtime;

namespace MatchmakingSystem.UI
{
    /// <summary>
    /// UI Toolkit implementation for the Lobby state, showing players, ready statuses, and starting the game.
    /// Requires a UIDocument component on the same GameObject.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class LobbyUI : MonoBehaviour
    {
        [Header("UI Toolkit Assets")]
        public VisualTreeAsset PlayerEntryTemplate; // Template with two labels: PlayerNameLabel and ReadyStatusLabel
        public MainMenuUI MainMenuToRestore; // Optional reference to show main menu again on leave

        private UIDocument _uiDocument;
        private VisualElement _root;
        private Button _readyButton;
        private Button _startGameButton;
        private Button _leaveButton;
        private ScrollView _playerListContainer;

        private void OnEnable()
        {
            // Subscriptions moved to Start to avoid execution order issues

            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            if (_root == null) return;

            // Query elements by name in UXML
            _readyButton = _root.Q<Button>("ReadyButton");
            _startGameButton = _root.Q<Button>("StartGameButton");
            _leaveButton = _root.Q<Button>("LeaveButton");
            _playerListContainer = _root.Q<ScrollView>("PlayerListContainer");

            // Initially hide the entire Lobby UI until a lobby is joined
            if (_root != null)
                _root.style.display = DisplayStyle.None;

            // Initially hide start game button
            if (_startGameButton != null)
                _startGameButton.style.display = DisplayStyle.None;
        }

        private void Start()
        {
            if (SteamLobbyManager.Instance != null)
            {
                SteamLobbyManager.Instance.OnLobbyLeft += OnLobbyLeft;
                SteamLobbyManager.Instance.OnLobbyCreated += OnLobbyEntered;
                SteamLobbyManager.Instance.OnLobbyJoined += OnLobbyEntered;
            }

            if (_readyButton != null)
                _readyButton.clicked += OnReadyClicked;

            if (_startGameButton != null)
                _startGameButton.clicked += OnStartGameClicked;

            if (_leaveButton != null)
                _leaveButton.clicked += OnLeaveClicked;
        }

        private void OnDisable()
        {
            if (SteamLobbyManager.Instance != null)
            {
                SteamLobbyManager.Instance.OnLobbyLeft -= OnLobbyLeft;
                SteamLobbyManager.Instance.OnLobbyCreated -= OnLobbyEntered;
                SteamLobbyManager.Instance.OnLobbyJoined -= OnLobbyEntered;
            }

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientStarted -= SubscribeToList;
            }

            if (_readyButton != null)
                _readyButton.clicked -= OnReadyClicked;

            if (_startGameButton != null)
                _startGameButton.clicked -= OnStartGameClicked;

            if (_leaveButton != null)
                _leaveButton.clicked -= OnLeaveClicked;

            UnsubscribeFromList();
        }

        private void Update()
        {
            if (_root == null || _startGameButton == null) return;

            // Only host can start game, and only if all ready
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
            {
                _startGameButton.style.display = DisplayStyle.Flex;
                _startGameButton.SetEnabled(NetworkLobbyState.Instance != null && NetworkLobbyState.Instance.AreAllPlayersReady());
            }
            else
            {
                _startGameButton.style.display = DisplayStyle.None;
            }
        }

        private void OnReadyClicked()
        {
            if (NetworkLobbyState.Instance != null)
            {
                NetworkLobbyState.Instance.ToggleReadyServerRpc();
            }
        }

        private void OnStartGameClicked()
        {
            if (NetworkLobbyState.Instance != null)
            {
                NetworkLobbyState.Instance.StartGame("SampleScene");
            }
        }

        private void OnLeaveClicked()
        {
            SteamLobbyManager.Instance?.LeaveLobby();
        }

        private void SubscribeToList()
        {
            if (NetworkLobbyState.Instance != null)
            {
                Debug.Log($"[LobbyUI] Subscribing to LobbyPlayers list. Current count: {NetworkLobbyState.Instance.LobbyPlayers.Count}");
                NetworkLobbyState.Instance.LobbyPlayers.OnListChanged += OnLobbyPlayersChanged;
                RefreshPlayerList(); // Initial draw
            }
            else
            {
                Invoke(nameof(SubscribeToList), 0.1f);
            }
        }

        private void UnsubscribeFromList()
        {
            if (NetworkLobbyState.Instance != null)
            {
                NetworkLobbyState.Instance.LobbyPlayers.OnListChanged -= OnLobbyPlayersChanged;
            }
        }

        private void OnLobbyPlayersChanged(Unity.Netcode.NetworkListEvent<LobbyPlayerState> changeEvent)
        {
            Debug.Log($"[LobbyUI] OnLobbyPlayersChanged fired. Event type: {changeEvent.Type}");
            RefreshPlayerList();
        }

        private void RefreshPlayerList()
        {
            if (_playerListContainer == null || PlayerEntryTemplate == null) return;

            // Clear existing
            _playerListContainer.Clear();

            if (NetworkLobbyState.Instance == null) return;

            Debug.Log($"[LobbyUI] Refreshing player list. Spawning {NetworkLobbyState.Instance.LobbyPlayers.Count} UI entries.");

            // Populate
            foreach (var player in NetworkLobbyState.Instance.LobbyPlayers)
            {
                var entry = PlayerEntryTemplate.Instantiate();
                var nameLabel = entry.Q<Label>("PlayerNameLabel");
                var statusLabel = entry.Q<Label>("ReadyStatusLabel");
                
                if (nameLabel != null)
                    nameLabel.text = player.PlayerName.ToString();

                if (statusLabel != null)
                {
                    statusLabel.text = player.IsReady ? "READY" : "NOT READY";
                    statusLabel.style.color = player.IsReady ? new StyleColor(Color.green) : new StyleColor(Color.red);
                }

                _playerListContainer.Add(entry);
            }
        }

        private void OnLobbyLeft()
        {
            // Hide this UI
            if (_root != null)
                _root.style.display = DisplayStyle.None;

            // Restore Main Menu UI if assigned
            if (MainMenuToRestore != null)
            {
                MainMenuToRestore.Show();
            }
        }

        private void OnLobbyEntered(Steamworks.Data.Lobby lobby)
        {
            Show();
            SubscribeToList();
        }
        
        /// <summary>
        /// Call to show the lobby UI manually (e.g. from MainMenu)
        /// </summary>
        public void Show()
        {
            if (_root != null)
                _root.style.display = DisplayStyle.Flex;
        }
    }
}
