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
    }
}