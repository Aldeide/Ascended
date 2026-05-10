using NUnit.Framework;
using Gameplay.Runtime.Interfaces;
using Gameplay.Runtime.Lobbies;

namespace Gameplay.Tests.Lobbies
{
    public class SteamLobbyTests
    {
        private class DummyPlayer : IPlayer
        {
            private string _name;
            private int _id;

            public DummyPlayer(int id, string name = "Player")
            {
                _id = id;
                _name = name;
            }

            public string GetPlayerName() => _name;
            public int GetPlayerId() => _id;
        }

        [Test]
        public void AddPlayer_AddsPlayerToLobby()
        {
            var lobby = new SteamLobby();
            var player = new DummyPlayer(1);

            lobby.AddPlayer(player);

            Assert.AreEqual(1, lobby.Players.Count);
            Assert.Contains(player, lobby.Players);
        }

        [Test]
        public void RemovePlayer_RemovesPlayerFromLobby()
        {
            var lobby = new SteamLobby();
            var player = new DummyPlayer(1);

            lobby.AddPlayer(player);
            Assert.AreEqual(1, lobby.Players.Count);

            lobby.RemovePlayer(player);

            Assert.AreEqual(0, lobby.Players.Count);
            Assert.IsFalse(lobby.Players.Contains(player));
        }
    }
}
