using UnityEngine;
using UnityEngine.UIElements;
using Steamworks.Data;
using MatchmakingSystem.Runtime;
using Systems.Core.Utilities;

namespace MatchmakingSystem.UI
{
    /// <summary>
    /// UI Toolkit implementation for the Main Menu to trigger Host/Join flows.
    /// Requires a UIDocument component on the same GameObject.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI Toolkit Assets")]
        public VisualTreeAsset LobbyEntryTemplate; // Template for each lobby item

        private UIDocument _uiDocument;
        private Button _hostButton;
        private Button _fetchLobbiesButton;
        private ScrollView _lobbyListContainer;
        private VisualElement _root;

        private void OnEnable()
        {
            // Move subscriptions to Start to avoid Awake race conditions

            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            if (_root == null) return;

            // Query elements by their name in the UXML
            _hostButton = _root.Q<Button>("HostButton");
            _fetchLobbiesButton = _root.Q<Button>("FetchLobbiesButton");
            _lobbyListContainer = _root.Q<ScrollView>("LobbyListContainer");
        }

        private void Start()
        {
            if (SteamLobbyManager.Instance != null)
            {
                SteamLobbyManager.Instance.OnLobbyListFetched += OnLobbyListFetched;
                SteamLobbyManager.Instance.OnLobbyJoined += OnLobbyJoined;
                SteamLobbyManager.Instance.OnLobbyCreated += OnLobbyCreated;
            }

            if (_hostButton != null)
                _hostButton.clicked += () => SteamLobbyManager.Instance?.HostLobby();
                
            if (_fetchLobbiesButton != null)
                _fetchLobbiesButton.clicked += () => SteamLobbyManager.Instance?.FetchLobbies();
        }

        private void OnDisable()
        {
            if (SteamLobbyManager.Instance != null)
            {
                SteamLobbyManager.Instance.OnLobbyListFetched -= OnLobbyListFetched;
                SteamLobbyManager.Instance.OnLobbyJoined -= OnLobbyJoined;
                SteamLobbyManager.Instance.OnLobbyCreated -= OnLobbyCreated;
            }

            if (_hostButton != null)
                _hostButton.clicked -= () => SteamLobbyManager.Instance?.HostLobby();
                
            if (_fetchLobbiesButton != null)
                _fetchLobbiesButton.clicked -= () => SteamLobbyManager.Instance?.FetchLobbies();
        }

        private void OnLobbyListFetched(Lobby[] lobbies)
        {
            if (_lobbyListContainer == null || LobbyEntryTemplate == null) return;

            // Clear existing
            _lobbyListContainer.Clear();

            // Populate
            foreach (var lobby in lobbies)
            {
                var entry = LobbyEntryTemplate.Instantiate();
                var btn = entry.Q<Button>("JoinButton");
                var label = entry.Q<Label>("LobbyNameLabel");
                
                if (label != null)
                    label.text = $"{StringUtilities.SanitizeForRichText(lobby.GetData("HostName"))}'s Lobby ({lobby.MemberCount}/{lobby.MaxMembers})";

                if (btn != null)
                    btn.clicked += () => SteamLobbyManager.Instance.JoinLobby(lobby);

                _lobbyListContainer.Add(entry);
            }
        }

        private void OnLobbyCreated(Lobby lobby)
        {
            // Hide this UI when transitioning to the lobby view
            if (_root != null)
                _root.style.display = DisplayStyle.None;
        }

        private void OnLobbyJoined(Lobby lobby)
        {
            // Hide this UI when transitioning to the lobby view
            if (_root != null)
                _root.style.display = DisplayStyle.None;
        }
        
        /// <summary>
        /// Can be called by LobbyUI to re-enable the Main Menu when leaving a lobby.
        /// </summary>
        public void Show()
        {
            if (_root != null)
                _root.style.display = DisplayStyle.Flex;
        }
    }
}
