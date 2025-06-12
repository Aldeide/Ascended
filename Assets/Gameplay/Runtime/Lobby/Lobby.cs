using System;
using System.Collections.Generic;
using Gameplay.Runtime.Players;

namespace Gameplay.Runtime.Lobby
{
    public class Lobby
    {
        public Guid LobbyGuid = Guid.NewGuid();
        public int MaxPlayers = 4;
        public List<Player> Players = new();
        
        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }
        
        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
        }
    }
}