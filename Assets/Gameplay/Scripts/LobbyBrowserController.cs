using System;
using Interface;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gameplay.Scripts
{
    public class LobbyBrowserController
    {
        private MainMenuController _mainMenuController;
        private VisualElement _lobbyBrowserVisualElement;
        
        // Buttons
        private Button _refreshButton;
        private Button _mainMenuButton;
        private Button _joinLobbyButton;
        
        // Lobby List
        private ListView _lobbyListView;
        private LobbyListController _lobbyListController;
        
        public void Initialise(VisualElement lobbyVisualElement, MainMenuController mainMenuController)
        {
            Debug.Log("Init LobbyBrowserController");
            _mainMenuController = mainMenuController;
            _lobbyBrowserVisualElement = lobbyVisualElement;
            _refreshButton = _lobbyBrowserVisualElement.Q<Button>("RefreshLobbiesButton");
            _mainMenuButton = _lobbyBrowserVisualElement.Q<Button>("ExitButton");
            _joinLobbyButton = _lobbyBrowserVisualElement.Q<Button>("JoinLobbyButton");
            
            _lobbyListView = _lobbyBrowserVisualElement.Q<ListView>("LobbyListView");
            _lobbyListController = new LobbyListController();
            _lobbyListController.InitialiseList(_lobbyBrowserVisualElement, _lobbyListView.itemTemplate);
            
            _refreshButton.clicked += RefreshLobbies;
            _mainMenuButton.clicked += _mainMenuController.ShowMainMenu;
        }

        private void RefreshLobbies()
        {
            Debug.Log("RefreshLobbies");
            _lobbyListController.GetLobbies();
        }
    }
}