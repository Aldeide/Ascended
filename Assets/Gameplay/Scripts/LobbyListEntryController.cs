using UnityEngine.UIElements;
using Systems.Core.Utilities;

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
            _lobbyLabel.text = StringUtilities.SanitizeForRichText(lobbyName);
        }
    }
}