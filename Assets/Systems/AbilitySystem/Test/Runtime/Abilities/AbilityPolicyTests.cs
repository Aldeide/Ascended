using AbilitySystem.Runtime.Abilities;
using static AbilitySystem.Test.Utilities.AbilityUtilities;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilityPolicyTests
    {
        [Test]
        public void AbilityPolicyTests_ClientOnly_ClientOrServer_ActivatesOnClient()
        {
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);

            var eventDispatched = false;
            clientAbilitySystem.AbilityManager.OnServerTryActivateAbilityRequested += (abilityName, key, data) =>
            {
                eventDispatched = true;
            };

            Assert.IsFalse(eventDispatched, "Client tried to request server ability activation but shouldn't have.");
            Assert.IsTrue(clientAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ClientOrServer_DoesNotActivateOnServerButRequestsClientActivation()
        {
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);

            var eventDispatched = false;
            serverAbilitySystem.AbilityManager.OnNotifyClientActivateAbility += (abilityName, data) =>
            {
                clientAbilitySystem.AbilityManager.TryActivateAbility(abilityName, data);
                eventDispatched = true;
            };

            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);

            Assert.IsTrue(eventDispatched, "Server did not notify client to activate ability.");
            Assert.IsFalse(serverAbilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive,
                "Ability is active on server.");
            Assert.IsTrue(clientAbilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive,
                "Ability isn't active on client.");
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ServerOnly_DoesNotActivateWhenRequestedByClient()
        {
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnly;
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);

            var eventDispatched = false;
            clientAbilitySystem.AbilityManager.OnServerTryActivateAbilityRequested += (abilityName, key, data) =>
            {
                eventDispatched = true;
            };

            Assert.IsFalse(clientAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName),
                "Ability has been activated by client but shouldn't have.");
            Assert.IsFalse(eventDispatched, "Client tried to request server ability activation but shouldn't have.");
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ServerOnly_ServerRequestsClientActivation()
        {
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnly;
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);

            var eventDispatched = false;
            serverAbilitySystem.AbilityManager.OnNotifyClientActivateAbility += (abilityName, data) =>
            {
                clientAbilitySystem.AbilityManager.ForceActivateAbility(abilityName, data);
                eventDispatched = true;
            };

            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);

            Assert.IsTrue(eventDispatched, "Server did not notify client to activate ability.");
            Assert.IsTrue(clientAbilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive,
                "Ability is inactive on client");
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ServerOnlyExecution_DoesNotActivateWhenRequestedByClient()
        {
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnlyExecution;
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);

            var eventDispatched = false;
            clientAbilitySystem.AbilityManager.OnServerTryActivateAbilityRequested += (abilityName, key, data) =>
            {
                eventDispatched = true;
            };

            Assert.IsFalse(clientAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName),
                "Ability has been activated by client but shouldn't have.");
            Assert.IsFalse(eventDispatched, "Client tried to request server ability activation but shouldn't have.");
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ServerOnlyExecution_ServerRequestsClientActivation()
        {
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnlyExecution;
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);

            var eventDispatched = false;
            serverAbilitySystem.AbilityManager.OnNotifyClientActivateAbility += (abilityName, data) =>
            {
                clientAbilitySystem.AbilityManager.ForceActivateAbility(abilityName, data);
                eventDispatched = true;
            };

            serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);

            Assert.IsTrue(eventDispatched, "Server did not notify client to activate ability.");
            Assert.IsTrue(clientAbilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive,
                "Ability is inactive on client");
        }

        [Test]
        public void AbilityPolicyTests_Server_ClientOrServer_ActivatesOnServer()
        {
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.Server;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);

            Assert.IsTrue(serverAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }

        [Test]
        public void AbilityPolicyTests_Server_ClientOrServer_ClientRequestsServerExecution()
        {
            var clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            var serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateTestAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.Server;
            abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            clientAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            serverAbilitySystem.AbilityManager.GrantAbility(abilityDefinition);

            var eventDispatched = false;
            var eventRaisedCount = 0;
            clientAbilitySystem.AbilityManager.OnServerTryUnpredictedAbilityRequested += (abilityName, data) =>
            {
                serverAbilitySystem.AbilityManager.TryActivateAbility(abilityName, data);
                eventDispatched = true;
                eventRaisedCount++;
            };

            Assert.IsTrue(clientAbilitySystem.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
            Assert.IsFalse(clientAbilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive);
            Assert.IsTrue(eventDispatched, "Client did not request server execution.");
            Assert.AreEqual(1, eventRaisedCount, "Client did not request server execution only once.");
            Assert.IsTrue(serverAbilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive,
                "Ability isn't active on server.");
        }
    }
}