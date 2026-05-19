using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.AbilityTasks
{
    /// <summary>
    /// Integration tests verifying WaitNetSyncTask synchronization modes (OnlyServerWait and BothWait).
    /// </summary>
    public class WaitNetSyncTaskTests : AbilitySystemTestBase
    {
        public class SyncTestAbility : Ability
        {
            public int SyncTaskFired = 0;

            public SyncTestAbility(AbilityDefinition definition, IAbilitySystem owner) 
                : base(definition, owner)
            {
            }

            protected override void ActivateAbility(AbilityData data)
            {
            }

            public void StartSyncTask(AbilityNetSyncType syncType)
            {
                var task = WaitNetSyncTask.CreateWaitNetSync(this, syncType);
                task.OnSyncFinished += () => SyncTaskFired++;
                task.ReadyForActivation();
            }

            public override void EndAbility()
            {
            }
        }

        public class SyncTestAbilityDefinition : AbilityDefinition
        {
            public SyncTestAbilityDefinition()
            {
                UniqueName = "SyncTestAbility";
                NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            }

            public override Type AbilityType() => typeof(SyncTestAbility);

            public override Ability ToAbility(IAbilitySystem owner)
            {
                return new SyncTestAbility(this, owner);
            }
        }

        protected override bool AddDefaultAttributes => false;

        protected override void InitializeMocks()
        {
            if (SourceMock == null) SourceMock = AbilitySystemUtilities.CreateMockClientAbilitySystem(AddDefaultAttributes);
            if (TargetMock == null) TargetMock = AbilitySystemUtilities.CreateMockServerAbilitySystem(AddDefaultAttributes);
            AbilitySystemUtilities.LinkAbilitySystems(SourceMock, TargetMock);
        }

        [Test]
        public void SyncTask_OnlyServerWait_CompletesClientImmediately_ServerCompletesOnRPC()
        {
            var def = new SyncTestAbilityDefinition();
            
            // Grant & activate ability on client and server
            var clientAbility = Source.AbilityManager.GrantAbility(def) as SyncTestAbility;
            var serverAbility = Target.AbilityManager.GrantAbility(def) as SyncTestAbility;

            Source.AbilityManager.TryActivateAbility(def.UniqueName);
            Target.AbilityManager.TryActivateAbility(def.UniqueName);

            Assert.IsTrue(clientAbility.IsActive);
            Assert.IsTrue(serverAbility.IsActive);

            var originalKey = clientAbility.PredictionKey;
            Assert.IsTrue(originalKey.IsValidKey());

            // Disable automatic sync key forwarding from client to server to control it manually in the test
            Source.ReplicationManager.OnServerSyncKeyReceived = null;

            // Activate OnlyServerWait task on client (client is local client, server is server)
            clientAbility.StartSyncTask(AbilityNetSyncType.OnlyServerWait);

            // Client should complete immediately
            Assert.AreEqual(1, clientAbility.SyncTaskFired);
            var clientNewKey = clientAbility.PredictionKey;
            Assert.AreNotEqual(originalKey.currentKey, clientNewKey.currentKey);
            Assert.AreEqual(originalKey.currentKey, clientNewKey.BaseKey);

            // Server should not complete yet because it hasn't started the task or processed the RPC
            Assert.AreEqual(0, serverAbility.SyncTaskFired);

            // Start sync task on server (it should wait)
            serverAbility.StartSyncTask(AbilityNetSyncType.OnlyServerWait);
            Assert.AreEqual(0, serverAbility.SyncTaskFired);

            // Send the key to server manually to simulate RPC routing
            Target.ReplicationManager.ProcessServerSyncKey(def.UniqueName, clientNewKey);

            // Now server should have received it and completed the task
            Assert.AreEqual(1, serverAbility.SyncTaskFired);
            Assert.AreEqual(clientNewKey.currentKey, serverAbility.PredictionKey.currentKey);
        }

        [Test]
        public void SyncTask_BothWait_CompletesOnlyWhenBothSidesReachAndSync()
        {
            var def = new SyncTestAbilityDefinition();
            var clientAbility = Source.AbilityManager.GrantAbility(def) as SyncTestAbility;
            var serverAbility = Target.AbilityManager.GrantAbility(def) as SyncTestAbility;

            Source.AbilityManager.TryActivateAbility(def.UniqueName);
            Target.AbilityManager.TryActivateAbility(def.UniqueName);

            // Capture the generated prediction key instead of automatically forwarding it
            PredictionKey capturedKey = default;
            Source.ReplicationManager.OnServerSyncKeyReceived = (name, key) => capturedKey = key;

            // Client starts BothWait task
            clientAbility.StartSyncTask(AbilityNetSyncType.BothWait);
            
            // Both wait: client should NOT complete immediately
            Assert.AreEqual(0, clientAbility.SyncTaskFired);

            // Server starts BothWait task
            serverAbility.StartSyncTask(AbilityNetSyncType.BothWait);
            Assert.AreEqual(0, serverAbility.SyncTaskFired);

            // Manually route the sync key to the server using the captured key
            Target.ReplicationManager.ProcessServerSyncKey(def.UniqueName, capturedKey);

            // Since linking is bidirectional and automatic in Linked mocks, 
            // once both started, they should both be completed!
            Assert.AreEqual(1, clientAbility.SyncTaskFired);
            Assert.AreEqual(1, serverAbility.SyncTaskFired);
            Assert.AreEqual(serverAbility.PredictionKey.currentKey, clientAbility.PredictionKey.currentKey);
        }
    }
}
