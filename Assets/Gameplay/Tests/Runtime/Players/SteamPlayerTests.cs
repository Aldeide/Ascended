using NUnit.Framework;
using Gameplay.Runtime.Players;
using Steamworks;

namespace Gameplay.Tests.Players
{
    [TestFixture]
    public class SteamPlayerTests
    {
        [Test]
        public void GetPlayerName_ReturnsCorrectName()
        {
            // Arrange
            string expectedName = "TestPlayer";
            SteamId expectedId = new SteamId();
            expectedId.Value = 12345;
            var player = new SteamPlayer(expectedName, expectedId);

            // Act
            string actualName = player.GetPlayerName();

            // Assert
            Assert.AreEqual(expectedName, actualName);
        }

        [Test]
        public void GetPlayerId_ReturnsCorrectId()
        {
            // Arrange
            string expectedName = "TestPlayer";
            SteamId expectedId = new SteamId();
            expectedId.Value = 12345;
            var player = new SteamPlayer(expectedName, expectedId);

            // Act
            int actualId = player.GetPlayerId();

            // Assert
            Assert.AreEqual((int)expectedId.AccountId, actualId);
        }
    }
}
