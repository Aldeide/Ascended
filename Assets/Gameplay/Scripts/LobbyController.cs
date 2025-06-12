using Gameplay.Runtime.Lobby;
using Unity.Netcode;
using UnityEngine.UIElements;

namespace Gameplay.Scripts
{
    public class LobbyController : NetworkBehaviour
    {
        private UIDocument _lobbyUI;
        private Lobby _lobby;
        private void Start()
        {
            _lobbyUI = GetComponent<UIDocument>();    
        }

        public void CreateLobby()
        {
            
        }
    }
}