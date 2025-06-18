using UnityEngine.UIElements;

namespace Gameplay.Scripts
{
    public class LobbyListEntryController
    {
        private Label _lobbyLabel;

        public void SetVisualElement(VisualElement visualElement)
        {
            _lobbyLabel = visualElement.Q<Label>("LobbyListEntryLabel");
            
        }

        public void SetLobbyEntryData(string lobbyName)
        {
            _lobbyLabel.name = lobbyName;
        }
    }
}