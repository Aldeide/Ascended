using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using Moq;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;

namespace AbilitySystem.Test.Runtime.Networking
{
    public class ServerClientInteractionTests
    {
        [Test]
        public void ServerClientInteraction_ClientTryActivateAbility_RequestsServerActivation()
        {
            // Arrange: Create a simulated client ability system.
            var clientSystem = CreateMockClientAbilitySystem();
            
            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.Interact";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            clientSystem.Object.AbilityManager.GrantAbility(abilityDef);

            // Variable to capture the event arguments emitted when the client calls TryActivateAbility.
            string requestedAbilityName = null;
            AbilitySystem.Runtime.Networking.PredictionKey requestedPredictionKey = default;
            
            // Subscribe to the event that replaces the old direct RPC logic.
            clientSystem.Object.ReplicationManager.OnServerAbilityActivationRequested += (name, key, data) =>
            {
                requestedAbilityName = name;
                requestedPredictionKey = key;
            };

            // Act: Client attempts to activate the ability locally.
            // Since it's a client, it should successfully predict locally and ask the server.
            clientSystem.Object.AbilityManager.TryActivateAbility("Test.Ability.Interact", new AbilityData());

            // Assert: Verify that the pure C# event was invoked for server activation, completely abstracted from Unity's RPCs!
            Assert.AreEqual("Test.Ability.Interact", requestedAbilityName, "Client did not emit the correct ability name to the server request map.");
            Assert.IsTrue(requestedPredictionKey.IsValidKey(), "Client did not generate a valid prediction key.");
        }

        [Test]
        public void ServerClientInteraction_ServerValidatesClientRequest_GrantsAndActivatesAbility()
        {
            // Arrange
            var serverSystem = CreateMockServerAbilitySystem();
            var clientSystem = CreateMockClientAbilitySystem();

            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.CrossSystem";
            abilityDef.Cost = null; // No cost for simplicity
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            
            clientSystem.Object.AbilityManager.GrantAbility(abilityDef);
            serverSystem.Object.AbilityManager.GrantAbility(abilityDef);

            // Connect the simulated systems directly without Netcode
            clientSystem.Object.ReplicationManager.OnServerAbilityActivationRequested += (name, key, data) =>
            {
                // This simulates the RPC flying across the network.
                // The server receives the key and validation data from the client, running its own TryActivateAbility routine safely.
                serverSystem.Object.ReplicationManager.ProcessServerAbilityActivation(name, key, data);
            };

            // Before activation: ability on the server should be inactive
            Assert.IsFalse(serverSystem.Object.AbilityManager.Abilities["Test.Ability.CrossSystem"].IsActive, 
                "Server ability should be inactive before client requests it.");

            // Act: Fire the client activation
            bool clientStartedPreidction = clientSystem.Object.AbilityManager.TryActivateAbility("Test.Ability.CrossSystem", new AbilityData());

            // Assert
            Assert.IsTrue(clientStartedPreidction, "Client failed to initially predict the activation locally.");
            
            // The magic moment: Does the server's ability read as active after validating the client's RPC event payload?
            Assert.IsTrue(serverSystem.Object.AbilityManager.Abilities["Test.Ability.CrossSystem"].IsActive, 
                "Server ability did not activate after receiving the abstract client event invocation.");
        }
    }
}
