using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine.UIElements;

namespace Gameplay.Scripts
{
    public class LobbyController : NetworkBehaviour
    {
        private UIDocument _lobbyUI;
        private ListView _lobbyListView;

        private Button _updateLobbyListButton;
        private LobbyListController _lobbyListController;
        private void Start()
        {
            _lobbyUI = GetComponent<UIDocument>();
            _lobbyListView = _lobbyUI.rootVisualElement.Q<ListView>("LobbyList");
            _lobbyListController = new LobbyListController();
            _lobbyListController.InitialiseList(_lobbyUI.rootVisualElement, _lobbyListView.itemTemplate);
            _updateLobbyListButton = _lobbyUI.rootVisualElement.Q<Button>("UpdateButton");
            _updateLobbyListButton.clicked+= UpdateLobbyList;
            Steamworks.SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
            Steamworks.SteamMatchmaking.CreateLobbyAsync(4);

        }

        public void OnLobbyCreated(Result result, Lobby lobby)
        {
            lobby.SetData("LobbyName", "TestLobby");
        }
        
        public async Task<Lobby?> CreateLobby()
        {
            var result = await Steamworks.SteamMatchmaking.CreateLobbyAsync(4);
            return result;
        }

        public void UpdateLobbyList()
        {
            _lobbyListController.GetLobbies();
        }

    }
}