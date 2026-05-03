using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Networking
{
    /// <summary>
    /// Unit tests for server-client interactions, verifying the core networking handshake during ability activation without relying on Unity Netcode.
    /// </summary>
    public class ServerClientInteractionTests : AbilitySystemTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            // Specifically setup Source as Client and Target as Server for interaction tests
            SourceMock = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            TargetMock = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            
            base.SetUp();
        }

        /// <summary>
        /// Verifies that when a client attempts to activate a predicted ability, it correctly emits a server activation request via the ReplicationManager.
        /// </summary>
        [Test]
        public void ServerClientInteractionTests_ClientActivation_RequestsServerActivationWithValidKey()
        {
            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.Interact";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            Source.AbilityManager.GrantAbility(abilityDef);

            string requestedAbilityName = null;
            PredictionKey requestedPredictionKey = default;
            
            // Subscribe to the event that replaces direct RPC logic
            Source.ReplicationManager.OnServerAbilityActivationRequested += (name, key, data) =>
            {
                requestedAbilityName = name;
                requestedPredictionKey = key;
            };

            // Client attempts to activate locally
            Source.AbilityManager.TryActivateAbility("Test.Ability.Interact", new AbilityData());

            Assert.AreEqual("Test.Ability.Interact", requestedAbilityName, "Client should have requested the specific ability name on the server");
            Assert.IsTrue(requestedPredictionKey.IsValidKey(), "Client should have generated and sent a valid prediction key");
        }

        /// <summary>
        /// Verifies that a server correctly processes a client's activation request, leading to the activation of the authoritative ability on the server.
        /// </summary>
        [Test]
        public void ServerClientInteractionTests_ServerReceiveRequest_ActivatesAuthoritativeAbility()
        {
            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.CrossSystem";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            
            Source.AbilityManager.GrantAbility(abilityDef);
            Target.AbilityManager.GrantAbility(abilityDef);

            // Connect the simulated systems
            AbilitySystemUtilities.LinkAbilitySystems(SourceMock, TargetMock);

            // Initial state check
            Assert.IsFalse(Target.AbilityManager.Abilities["Test.Ability.CrossSystem"].IsActive, "Server ability should be inactive initially");

            // Client activates
            bool clientPredictionStarted = Source.AbilityManager.TryActivateAbility("Test.Ability.CrossSystem", new AbilityData());

            Assert.IsTrue(clientPredictionStarted, "Client should have successfully started local prediction");
            Assert.IsTrue(Target.AbilityManager.Abilities["Test.Ability.CrossSystem"].IsActive, "Server ability should have activated after processing the client request");
        }
    }
}
