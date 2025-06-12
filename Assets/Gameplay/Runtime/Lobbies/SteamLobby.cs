using System;
using System.Collections.Generic;
using Gameplay.Runtime.Interfaces;
using Gameplay.Runtime.Players;

namespace Gameplay.Runtime.Lobbies
{
    public class SteamLobby : ILobby
    {
        public Guid LobbyGuid = Guid.NewGuid();
        public int MaxPlayers = 4;
        public List<IPlayer> Players = new();
        
        public void AddPlayer(IPlayer player)
        {
            Players.Add(player);
        }
        
        public void RemovePlayer(IPlayer player)
        {
            Players.Remove(player);
        }
    }
}