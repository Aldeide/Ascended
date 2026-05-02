using NUnit.Framework;
using Gameplay.Runtime.Lobbies;
using Gameplay.Runtime.Interfaces;

namespace Gameplay.Tests.Runtime.Lobbies
{
    public class SteamLobbyTests
    {
        private class MockPlayer : IPlayer
        {
            public string Name { get; set; } = string.Empty;
            public int Id { get; set; }

            public string GetPlayerName() => Name;
            public int GetPlayerId() => Id;
        }

        [Test]
        public void AddPlayer_AddsPlayerToLobby()
        {
            var lobby = new SteamLobby();
            var player = new MockPlayer { Name = "TestPlayer", Id = 1 };

            lobby.AddPlayer(player);

            Assert.That(lobby.Players, Does.Contain(player));
            Assert.That(lobby.Players.Count, Is.EqualTo(1));
        }

        [Test]
        public void RemovePlayer_RemovesPlayerFromLobby()
        {
            var lobby = new SteamLobby();
            var player = new MockPlayer { Name = "TestPlayer", Id = 1 };
            lobby.AddPlayer(player);

            lobby.RemovePlayer(player);

            Assert.That(lobby.Players, Does.Not.Contain(player));
            Assert.That(lobby.Players.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemovePlayer_DoesNothing_WhenPlayerNotInLobby()
        {
            var lobby = new SteamLobby();
            var player1 = new MockPlayer { Name = "Player1", Id = 1 };
            var player2 = new MockPlayer { Name = "Player2", Id = 2 };
            lobby.AddPlayer(player1);

            lobby.RemovePlayer(player2);

            Assert.That(lobby.Players, Does.Contain(player1));
            Assert.That(lobby.Players, Does.Not.Contain(player2));
            Assert.That(lobby.Players.Count, Is.EqualTo(1));
        }
    }
}
