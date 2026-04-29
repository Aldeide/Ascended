using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using Moq;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.AbilityTasks
{
    public class AbilityTaskIntegrationTests
    {
        private Mock<IAbilitySystem> _mockClientSystem;
        private Mock<IAbilitySystem> _mockServerSystem;
        private TestAbility _clientAbility;
        private TestAbility _serverAbility;

        [SetUp]
        public void Setup()
        {
            _mockClientSystem = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            _mockServerSystem = AbilitySystemUtilities.CreateMockServerAbilitySystem();

            var definition = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            definition.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;

            _clientAbility = new TestAbility(definition, _mockClientSystem.Object);
            _serverAbility = new TestAbility(definition, _mockServerSystem.Object);
        }

        [Test]
        public void WaitTargetData_PredictedExecution_RunsOnClientAndServer()
        {
            // Start on client
            _clientAbility.IsActive = true;
            var clientTask = WaitTargetDataTask.CreateWaitTargetData(_clientAbility);
            bool clientFinished = false;
            clientTask.OnTargetDataReceived += (d) => clientFinished = true;
            clientTask.ReadyForActivation();

            // Start on server
            _serverAbility.IsActive = true;
            var serverTask = WaitTargetDataTask.CreateWaitTargetData(_serverAbility);
            bool serverFinished = false;
            serverTask.OnTargetDataReceived += (d) => serverFinished = true;
            serverTask.ReadyForActivation();

            Assert.IsTrue(clientTask.IsActive);
            Assert.IsTrue(serverTask.IsActive);

            // Simulate client confirming data
            clientTask.ConfirmTargetData(new AbilitySystem.Runtime.Abilities.Targeting.TargetDataHandle());
            Assert.IsTrue(clientFinished);
            Assert.IsFalse(clientTask.IsActive);

            // Server is still waiting
            Assert.IsFalse(serverFinished);
            Assert.IsTrue(serverTask.IsActive);

            // Simulate server receiving RPC and confirming data
            serverTask.ConfirmTargetData(new AbilitySystem.Runtime.Abilities.Targeting.TargetDataHandle());
            Assert.IsTrue(serverFinished);
            Assert.IsFalse(serverTask.IsActive);
        }

        [Test]
        public void Rollback_CleansUpClientTasks()
        {
            _clientAbility.IsActive = true;
            var clientTask = WaitDelayTask.CreateWaitDelay(_clientAbility, 5.0f);
            clientTask.ReadyForActivation();

            Assert.IsTrue(clientTask.IsActive);

            // Simulate server rejecting the ability, triggering a rollback
            _clientAbility.TryCancelAbility();

            Assert.IsFalse(clientTask.IsActive);
        }
    }
}
