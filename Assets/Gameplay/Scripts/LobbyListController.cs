using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gameplay.Scripts
{
    public class LobbyListController
    {
        private VisualTreeAsset _listEntryTemplate;
        private ListView _lobbyListView;
        private List<string> _lobbyList;

        private Action<Task<Lobby[]>> OnLobbiesFetched;
        
        public void InitialiseList(VisualElement root, VisualTreeAsset listEntryTemplate)
        {
            _listEntryTemplate = listEntryTemplate;
            _lobbyListView = root.Q<ListView>("LobbyList");
            OnLobbiesFetched += LobbiesFetched;
        }

        public void GetLobbies()
        {
            _lobbyList = new List<string>();
            var lobbyQuery = Steamworks.SteamMatchmaking.LobbyList;
            lobbyQuery.RequestAsync().ContinueWith(OnLobbiesFetched);
        }

        public void LobbiesFetched(Task<Lobby[]> lobbies)
        {
            Debug.Log("Fetched: " + lobbies.Result.Length);
            foreach (var lobby in  lobbies.Result)
            {
                foreach (var data in lobby.Data)
                {
                    Debug.Log("Data key: " + data.Key + " value: " + data.Value);
                }
                
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
            
            _lobbyListView.itemsSource = _lobbyList;
            _lobbyListView.Refresh();
        }
    }
}