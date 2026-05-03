using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.Targeting;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.AbilityTasks
{
    /// <summary>
    /// Integration tests for AbilityTasks, verifying how tasks coordinate state across client and server during predicted ability execution.
    /// </summary>
    public class AbilityTaskIntegrationTests : AbilitySystemTestBase
    {
        private TestAbility _clientAbility;
        private TestAbility _serverAbility;

        [SetUp]
        public override void SetUp()
        {
            // Setup source as client and target as server
            SourceMock = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            TargetMock = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            
            base.SetUp();

            var definition = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            definition.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;

            _clientAbility = new TestAbility(definition, Source);
            _serverAbility = new TestAbility(definition, Target);
        }

        /// <summary>
        /// Verifies that WaitTargetDataTask correctly activates and completes independently on both client and server during a predicted ability flow.
        /// </summary>
        [Test]
        public void AbilityTaskIntegrationTests_WaitTargetData_CompletesIndependentlyOnClientAndServer()
        {
            // Start task on client
            _clientAbility.IsActive = true;
            var clientTask = WaitTargetDataTask.CreateWaitTargetData(_clientAbility);
            bool clientFinished = false;
            clientTask.OnTargetDataReceived += (d) => clientFinished = true;
            clientTask.ReadyForActivation();

            // Start task on server
            _serverAbility.IsActive = true;
            var serverTask = WaitTargetDataTask.CreateWaitTargetData(_serverAbility);
            bool serverFinished = false;
            serverTask.OnTargetDataReceived += (d) => serverFinished = true;
            serverTask.ReadyForActivation();

            Assert.IsTrue(clientTask.IsActive, "Client task should be active");
            Assert.IsTrue(serverTask.IsActive, "Server task should be active");

            // Client confirms data (e.g., local user input)
            clientTask.ConfirmTargetData(new TargetDataHandle());
            Assert.IsTrue(clientFinished, "Client task should have finished after confirming data");
            Assert.IsFalse(clientTask.IsActive, "Client task should no longer be active");

            // Server should still be waiting for the RPC
            Assert.IsFalse(serverFinished, "Server task should not have finished yet");
            Assert.IsTrue(serverTask.IsActive, "Server task should still be active");

            // Server receives data (simulated RPC confirmation)
            serverTask.ConfirmTargetData(new TargetDataHandle());
            Assert.IsTrue(serverFinished, "Server task should have finished after receiving data");
            Assert.IsFalse(serverTask.IsActive, "Server task should no longer be active");
        }

        /// <summary>
        /// Verifies that any active tasks on the client are correctly terminated when a predicted ability is rolled back due to server denial.
        /// </summary>
        [Test]
        public void AbilityTaskIntegrationTests_Rollback_AutomaticallyTerminatesClientTasks()
        {
            _clientAbility.IsActive = true;
            var clientTask = WaitDelayTask.CreateWaitDelay(_clientAbility, 5.0f);
            clientTask.ReadyForActivation();

            Assert.IsTrue(clientTask.IsActive, "Task should be active on client initially");

            // Simulate server rejecting the ability, triggering a local cancellation/rollback
            _clientAbility.TryCancelAbility();

            Assert.IsFalse(clientTask.IsActive, "Client task should have been deactivated during ability rollback");
        }

        #region Helper Classes
        private class TestAbility : Ability
        {
            public TestAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner) { }
            protected override void ActivateAbility(AbilityData data) { }
            public override void EndAbility() { }
            public new bool IsActive { get => base.IsActive; set => base.IsActive = value; }
        }
        #endregion
    }
}
