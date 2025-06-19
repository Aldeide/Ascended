using Gameplay.Scripts;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface
{
    public class MainMenuController : MonoBehaviour
    {
        public UIDocument SettingsMenu;
        public UIDocument _document;
        
        // Buttons
        private Button _playButton;
        private Button _settingsButton;
        private Button _hostButton;
        
        private VisualElement _mainMenu;
        private TemplateContainer _settingsTemplate;
        private TemplateContainer _playTemplate;
        
        private LobbyBrowserController _lobbyBrowserController;
        
        private void Start()
        {
            _document = GetComponent<UIDocument>();
            //SettingsMenu.rootVisualElement.visible = false;
            _playButton = _document.rootVisualElement.Q<Button>("PlayButton");
            _settingsButton = _document.rootVisualElement.Q<Button>("SettingsButton");
            _hostButton = _document.rootVisualElement.Q<Button>("HostButton");
            
            _playButton.clicked += OnPlayButtonClicked;
            _settingsButton.clicked += OnSettingsButtonClicked;
            _hostButton.clicked += OnHostButtonClicked;
            
            _mainMenu = _document.rootVisualElement.Q<VisualElement>("MainMenuTemplate");
            _settingsTemplate = _document.rootVisualElement.Q<TemplateContainer>("SettingsTemplate");
            _settingsTemplate.visible = false;
            _playTemplate = _document.rootVisualElement.Q<TemplateContainer>("PlayMenu");
            _playTemplate.visible = false;
            _lobbyBrowserController = new LobbyBrowserController();
            _lobbyBrowserController.Initialise(_playTemplate, this);
            _mainMenu.visible = true;
            
            SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;

        }

        public void ShowMainMenu()
        {
            _mainMenu.visible = true;
            _settingsTemplate.visible = false;
            _playTemplate.visible = false;
        }

        private void OnPlayButtonClicked()
        {
            Debug.Log("PlayButtonClicked");
            _mainMenu.visible = false;
            _settingsTemplate.visible = false;
            _playTemplate.visible = true;
        }
        
        private void OnSettingsButtonClicked()
        {
            _mainMenu.visible = false;
            _settingsTemplate.visible = true;
            Debug.Log("SettingsButtonClicked");
            //_document.rootVisualElement.visible = false;
        }

        private void OnHostButtonClicked()
        {
            SteamMatchmaking.CreateLobbyAsync(4);
        }

        public void OnLobbyCreated(Result result, Lobby lobby)
        {
            lobby.SetData("LobbyName", "TestLobby");
            Debug.Log("Lobby created");
        }
        
        private void OnQuitButtonClicked()
        {
            Debug.Log("QuitButtonClicked");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}