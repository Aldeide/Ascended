using Gameplay.Runtime.Interfaces;
using Gameplay.Runtime.Players;
using UnityEngine;
using Steamworks;

namespace Gameplay.Scripts
{
    public class GameController : MonoBehaviour
    {
        public IPlayer Player;
        public static GameController Instance
        {
            get
            {
                if(!_instance)
                {
                    _instance = FindFirstObjectByType<GameController>();
                }

                return _instance;
            }
        }
        
        private static GameController _instance;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            try
            {
                // TODO(antho): Uncomment once we have a steam app id.
                // Steamworks.SteamClient.Init( 252490, true );
            }
            catch ( System.Exception e )
            {
                // Something went wrong! Steam is closed?
            }
        }
        
        private void Start()
        {
            var playerName = "DefaultPlayer";
            uint playerId = 0;
            if (SteamClient.IsValid)
            {
                playerName = SteamClient.Name;
                playerId = SteamClient.SteamId.AccountId;
            }
            Player = new SteamPlayer(playerName, SteamClient.SteamId);
        }

        private void OnDisable()
        {
            SteamClient.Shutdown();
        }
    }
}