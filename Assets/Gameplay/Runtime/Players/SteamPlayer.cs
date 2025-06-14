using Gameplay.Runtime.Interfaces;
using Steamworks;

namespace Gameplay.Runtime.Players
{
    public class SteamPlayer : IPlayer
    {
        private readonly string _name;
        private SteamId _steamId;
        public SteamPlayer(string name, SteamId playerId)
        {
            _name = name;
            _steamId = playerId;
        }
        
        public string GetPlayerName()
        {
            return _name;
        }

        public int GetPlayerId()
        {
            return (int)_steamId.AccountId;
        }
    }
}