using NUnit.Framework;
using Moq;
using Gameplay.Runtime.Interfaces;
using Gameplay.Runtime.Lobbies;
using System.Linq;

namespace Gameplay.Tests.Lobbies
{
    public class SteamLobbyTests
    {
        [Test]
        public void AddPlayer_AddsPlayerToLobby()
        {
            var lobby = new SteamLobby();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(p => p.GetPlayerId()).Returns(1);

            lobby.AddPlayer(playerMock.Object);

            Assert.AreEqual(1, lobby.Players.Count);
            Assert.Contains(playerMock.Object, lobby.Players);
        }

        [Test]
        public void RemovePlayer_RemovesPlayerFromLobby()
        {
            var lobby = new SteamLobby();
            var playerMock = new Mock<IPlayer>();
            playerMock.Setup(p => p.GetPlayerId()).Returns(1);

            lobby.AddPlayer(playerMock.Object);
            Assert.AreEqual(1, lobby.Players.Count);

            lobby.RemovePlayer(playerMock.Object);

            Assert.AreEqual(0, lobby.Players.Count);
            Assert.IsFalse(lobby.Players.Contains(playerMock.Object));
        }
    }
}
