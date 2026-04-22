using AbilitySystem.Runtime.Abilities;
using static AbilitySystem.Test.Utilities.AbilityUtilities;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using NUnit.Framework;
using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilityPolicyTests
    {
        private IAbilitySystem _clientAbilitySystem;
        private IAbilitySystem _serverAbilitySystem;
        private AbilityDefinition _abilityDefinition;

        [SetUp]
        public void SetUp()
        {
            _clientAbilitySystem = CreateMockClientAbilitySystem().Object;
            _serverAbilitySystem = CreateMockServerAbilitySystem().Object;
            _abilityDefinition = CreateTestAbilityDefinition();
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ClientOrServer_ActivatesOnClient()
        {
            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            _abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            _clientAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);
            _serverAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            var eventDispatched = false;
            _clientAbilitySystem.AbilityManager.OnServerTryActivateAbilityRequested += (abilityName, key, data) =>
            {
                eventDispatched = true;
            };

            Assert.IsFalse(eventDispatched, "Client tried to request server ability activation but shouldn't have.");
            Assert.IsTrue(_clientAbilitySystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName));
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ClientOrServer_DoesNotActivateOnServerButRequestsClientActivation()
        {
            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            _abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            _clientAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);
            _serverAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            var eventDispatched = false;
            _serverAbilitySystem.AbilityManager.OnNotifyClientActivateAbility += (abilityName, data) =>
            {
                _clientAbilitySystem.AbilityManager.TryActivateAbility(abilityName, data);
                eventDispatched = true;
            };

            _serverAbilitySystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName);

            Assert.IsTrue(eventDispatched, "Server did not notify client to activate ability.");
            Assert.IsFalse(_serverAbilitySystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive,
                "Ability is active on server.");
            Assert.IsTrue(_clientAbilitySystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive,
                "Ability isn't active on client.");
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ServerOnly_DoesNotActivateWhenRequestedByClient()
        {
            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            _abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnly;
            _clientAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);
            _serverAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            var eventDispatched = false;
            _clientAbilitySystem.AbilityManager.OnServerTryActivateAbilityRequested += (abilityName, key, data) =>
            {
                eventDispatched = true;
            };

            Assert.IsFalse(_clientAbilitySystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName),
                "Ability has been activated by client but shouldn't have.");
            Assert.IsFalse(eventDispatched, "Client tried to request server ability activation but shouldn't have.");
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ServerOnly_ServerRequestsClientActivation()
        {
            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            _abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnly;
            _clientAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);
            _serverAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            var eventDispatched = false;
            _serverAbilitySystem.AbilityManager.OnNotifyClientActivateAbility += (abilityName, data) =>
            {
                _clientAbilitySystem.AbilityManager.ForceActivateAbility(abilityName, data);
                eventDispatched = true;
            };

            _serverAbilitySystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName);

            Assert.IsTrue(eventDispatched, "Server did not notify client to activate ability.");
            Assert.IsTrue(_clientAbilitySystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive,
                "Ability is inactive on client");
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ServerOnlyExecution_DoesNotActivateWhenRequestedByClient()
        {
            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            _abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnlyExecution;
            _clientAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);
            _serverAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            var eventDispatched = false;
            _clientAbilitySystem.AbilityManager.OnServerTryActivateAbilityRequested += (abilityName, key, data) =>
            {
                eventDispatched = true;
            };

            Assert.IsFalse(_clientAbilitySystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName),
                "Ability has been activated by client but shouldn't have.");
            Assert.IsFalse(eventDispatched, "Client tried to request server ability activation but shouldn't have.");
        }

        [Test]
        public void AbilityPolicyTests_ClientOnly_ServerOnlyExecution_ServerRequestsClientActivation()
        {
            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            _abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnlyExecution;
            _clientAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);
            _serverAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            var eventDispatched = false;
            _serverAbilitySystem.AbilityManager.OnNotifyClientActivateAbility += (abilityName, data) =>
            {
                _clientAbilitySystem.AbilityManager.ForceActivateAbility(abilityName, data);
                eventDispatched = true;
            };

            _serverAbilitySystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName);

            Assert.IsTrue(eventDispatched, "Server did not notify client to activate ability.");
            Assert.IsTrue(_clientAbilitySystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive,
                "Ability is inactive on client");
        }

        [Test]
        public void AbilityPolicyTests_Server_ClientOrServer_ActivatesOnServer()
        {
            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.Server;
            _abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            _serverAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            Assert.IsTrue(_serverAbilitySystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName));
        }

        [Test]
        public void AbilityPolicyTests_Server_ClientOrServer_ClientRequestsServerExecution()
        {
            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.Server;
            _abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ClientOrServer;
            _clientAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);
            _serverAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            var eventDispatched = false;
            var eventRaisedCount = 0;
            _clientAbilitySystem.AbilityManager.OnServerTryUnpredictedAbilityRequested += (abilityName, data) =>
            {
                _serverAbilitySystem.AbilityManager.TryActivateAbility(abilityName, data);
                eventDispatched = true;
                eventRaisedCount++;
            };

            Assert.IsTrue(_clientAbilitySystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName));
            Assert.IsFalse(_clientAbilitySystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive);
            Assert.IsTrue(eventDispatched, "Client did not request server execution.");
            Assert.AreEqual(1, eventRaisedCount, "Client did not request server execution only once.");
            Assert.IsTrue(_serverAbilitySystem.AbilityManager.Abilities[_abilityDefinition.UniqueName].IsActive,
                "Ability isn't active on server.");
        }

        [Test]
        public void AbilityPolicyTests_Server_ServerOnly_ActivatesOnServer()
        {
            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.Server;
            _abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnly;
            _serverAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            var isEventRaised = false;
            _serverAbilitySystem.AbilityManager.OnNotifyClientActivateAbility += (abilityName, data) =>
            {
                isEventRaised = true;
            };
            Assert.IsFalse(isEventRaised, "Server tried to notify client to activate ability but shouldn't have.");
            ;
            Assert.IsTrue(_serverAbilitySystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName));
        }

        [Test]
        public void AbilityPolicyTests_Server_ServerOnly_DoesNotActivatesOnClient()
        {
            _abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.Server;
            _abilityDefinition.NetworkSecurityPolicy = AbilityNetworkSecurityPolicy.ServerOnly;
            _clientAbilitySystem.AbilityManager.GrantAbility(_abilityDefinition);

            var isEventUnpredictedRaised = false;
            _serverAbilitySystem.AbilityManager.OnServerTryUnpredictedAbilityRequested += (abilityName, data) =>
            {
                isEventUnpredictedRaised = true;
            };
            var isEventPredictedRaised = false;
            _serverAbilitySystem.AbilityManager.OnServerTryActivateAbilityRequested += (abilityName, key, data) =>
            {
                isEventPredictedRaised = true;
            };
            Assert.IsFalse(isEventUnpredictedRaised,
                "Client tried to request server activation for an ability that is ServerOnly");
            Assert.IsFalse(isEventUnpredictedRaised,
                "Client tried to request server predicted activation for an ability that is ServerOnly");
            Assert.IsFalse(_serverAbilitySystem.AbilityManager.TryActivateAbility(_abilityDefinition.UniqueName));
        }
    }
}