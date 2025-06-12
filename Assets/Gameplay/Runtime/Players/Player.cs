using Steamworks;

namespace Gameplay.Runtime.Players
{
    public class Player
    {
        public string Name;
        public SteamId SteamId;
        public Player(string name, SteamId playerId)
        {
            Name = name;
            SteamId = playerId;
        }
    }
}