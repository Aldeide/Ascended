using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

namespace AbilitySystem.Test.Runtime.Networking
{
    [TestFixture]
    public class EffectReplicationTests : AbilitySystemSyncTestBase
    {
        private EffectDefinition _testEffectDef;

        public override void Setup()
        {
            base.Setup();

            _testEffectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            _testEffectDef.name = "TestEffect";
            _testEffectDef.DurationType = EffectDurationType.Infinite;
            
            DataManager.Effects.Add(_testEffectDef.name, _testEffectDef);
        }

        [Test]
        public void ServerRemovesEffect_ClientIsNotifiedAndRemovesIt()
        {
            // 1. Add effect to server
            var serverEffect = _testEffectDef.ToEffect(ServerSystem, ServerSystem);
            ServerSystem.EffectManager.AddEffectFromServer(serverEffect);
            
            // Wait, LinkSystems should have handled the notification and added it to client
            // But we need to make sure the client actually has it.
            Assert.IsTrue(ClientSystem.EffectManager.Effects.Any(e => e.Definition.name == "TestEffect"), 
                "Client should have received the effect from server via LinkSystems");

            // 2. Remove effect from server
            ServerSystem.EffectManager.RemoveEffect(serverEffect);

            // 3. Verify client removed it
            Assert.IsFalse(ClientSystem.EffectManager.Effects.Any(e => e.Definition.name == "TestEffect"), 
                "Client should have removed the effect after server notification");
        }

        [Test]
        public void ClientAttemptToRemoveEffect_DoesNotNotifyServer()
        {
            // Only server should be authoritative over effect removal notifications
            
            // 1. Add effect to both (manually to bypass replication for setup if needed, but let's use replication)
            var serverEffect = _testEffectDef.ToEffect(ServerSystem, ServerSystem);
            ServerSystem.EffectManager.AddEffectFromServer(serverEffect);
            
            Assert.IsTrue(ClientSystem.EffectManager.Effects.Any(e => e.Definition.name == "TestEffect"));

            // 2. Client removes its own local copy (simulating a bug or predicted removal)
            var clientEffect = ClientSystem.EffectManager.Effects.First(e => e.Definition.name == "TestEffect");
            
            // We need to verify that this DOES NOT trigger OnNotifyClientsEffectRemoved on the clientRepl
            bool notificationFired = false;
            ClientSystem.ReplicationManager.OnNotifyClientsEffectRemoved += (name) => notificationFired = true;

            ClientSystem.EffectManager.RemoveEffect(clientEffect);

            // 3. Verify
            Assert.IsFalse(notificationFired, "Client should NOT fire replication events for effect removal");
            Assert.IsTrue(ServerSystem.EffectManager.Effects.Any(e => e.Definition.name == "TestEffect"), 
                "Server effect should still exist because client removal isn't authoritative");
        }
        [Test]
        public void ClientPredictsEffect_ServerReplicatesSameEffect_ShouldNotDuplicate()
        {
            // 1. Client predicts effect (adds it locally)
            var clientEffect = _testEffectDef.ToEffect(ClientSystem, ClientSystem);
            ClientSystem.EffectManager.AddEffect(clientEffect);
            
            Assert.AreEqual(1, ClientSystem.EffectManager.Effects.Count, "Client should have 1 effect initially");

            // 2. Server adds same effect and replicates to client
            var serverEffect = _testEffectDef.ToEffect(ServerSystem, ServerSystem);
            ServerSystem.EffectManager.AddEffectFromServer(serverEffect);
            
            // LinkSystems calls AddEffectFromServer on client
            
            // 3. Verify client state
            // If it's duplicated, count will be 2.
            Assert.AreEqual(1, ClientSystem.EffectManager.Effects.Count, 
                "Client should NOT have duplicate effects of the same name from server if one already exists locally");
        }
    }
}
