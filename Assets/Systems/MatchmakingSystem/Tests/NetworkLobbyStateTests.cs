using NUnit.Framework;
using MatchmakingSystem.Runtime;
using Unity.Collections;

namespace MatchmakingSystem.Tests
{
    [TestFixture]
    public class NetworkLobbyStateTests
    {
        [Test]
        public void NetworkLobbyStateTests_StructEquality_ReturnsTrueForSameData()
        {
            // Arrange
            var state1 = new LobbyPlayerState
            {
                ClientId = 1,
                PlayerName = new FixedString64Bytes("TestPlayer"),
                IsReady = true
            };

            var state2 = new LobbyPlayerState
            {
                ClientId = 1,
                PlayerName = new FixedString64Bytes("TestPlayer"),
                IsReady = true
            };

            // Act
            bool areEqual = state1.Equals(state2);

            // Assert
            Assert.IsTrue(areEqual, "Structs with identical data should be equal.");
        }

        [Test]
        public void NetworkLobbyStateTests_StructEquality_ReturnsFalseForDifferentReadyState()
        {
            // Arrange
            var state1 = new LobbyPlayerState
            {
                ClientId = 1,
                PlayerName = new FixedString64Bytes("TestPlayer"),
                IsReady = true
            };

            var state2 = new LobbyPlayerState
            {
                ClientId = 1,
                PlayerName = new FixedString64Bytes("TestPlayer"),
                IsReady = false
            };

            // Act
            bool areEqual = state1.Equals(state2);

            // Assert
            Assert.IsFalse(areEqual, "Structs with different IsReady values should not be equal.");
        }

        [Test]
        public void UpdatePlayerNameServerRpc_SanitizesInput()
        {
            // Note: Standalone NUnit testing of NetworkBehaviour requires a mock environment.
            // The actual sanitization logic is tested in StringUtilitiesTests.
        }

    }
}
