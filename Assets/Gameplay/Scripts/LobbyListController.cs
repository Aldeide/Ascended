using System.Collections.Generic;
using Steamworks.Data;
using UnityEngine.UIElements;

namespace Gameplay.Scripts
{
    public class LobbyListController
    {
        private VisualTreeAsset _listEntryTemplate;
        private ListView _lobbyListView;
        private List<string> _lobbyList;

        public void InitialiseList(VisualElement root, VisualTreeAsset listEntryTemplate)
        {
            _listEntryTemplate = listEntryTemplate;
            _lobbyListView = root.Q<ListView>("LobbyList");
        }

        public void GetLobbies()
        {
            _lobbyList = new List<string>();
            var lobbyQuery = Steamworks.SteamMatchmaking.LobbyList;
            var lobbies = lobbyQuery.RequestAsync().Result;
            foreach (var lobby in  lobbies)
            {
               _lobbyList.Add(lobby.GetData("LobbyName")); 
            }
            FillList();
        }

        private void FillList()
        {
            _lobbyListView.makeItem = () =>
            {
                var newListEntry = _listEntryTemplate.Instantiate();
                var newListEntryController = new LobbyListEntryController();
                newListEntry.userData = newListEntryController;
                newListEntryController.SetVisualElement(newListEntry);
                return newListEntry;
            };
            
            _lobbyListView.bindItem = (item, index) =>
            {
                (item.userData as LobbyListEntryController)?.SetLobbyEntryData(_lobbyList[index]);
            };
        }
    }
}