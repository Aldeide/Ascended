using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Modifiers;
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

        [Test]
        public void ServerAddsStackToEffect_ClientReplicatesStackCount()
        {
            // Add TestAttributeSet to both server and client to verify attribute modifications
            var serverAttributes = new TestAttributeSet(ServerSystem);
            ServerSystem.AttributeSetManager.AddAttributeSet(typeof(TestAttributeSet), serverAttributes);
            var clientAttributes = new TestAttributeSet(ClientSystem);
            ClientSystem.AttributeSetManager.AddAttributeSet(typeof(TestAttributeSet), clientAttributes);

            // 1. Configure test effect to allow stacking and modify health
            _testEffectDef.EffectStack = new EffectStack 
            { 
                EffectStackType = EffectStackType.AggregateByTarget,
                MaxStacks = 3
            };
            
            var floatModifier = new FloatModifier
            {
                AttributeName = "TestAttributeSet.Health",
                Operation = EffectOperation.Additive,
                ModifierMagnitude = 10f
            };
            _testEffectDef.Modifiers = new Modifier[] { floatModifier };

            // 2. Add first application on server
            var serverEffect = _testEffectDef.ToEffect(ServerSystem, ServerSystem);
            ServerSystem.EffectManager.AddEffectFromServer(serverEffect);
            
            // Verify client got it with 1 stack
            Assert.IsTrue(ClientSystem.EffectManager.Effects.Any(e => e.Definition.name == "TestEffect"), 
                "Client should have received the effect");
            var clientEffect = ClientSystem.EffectManager.Effects.First(e => e.Definition.name == "TestEffect");
            Assert.AreEqual(1, clientEffect.NumStacks, "Client effect stack count should be 1 initially");
            
            // Client attributes should have recalculation run with 1 stack (+10 health)
            ClientSystem.AttributeSetManager.UpdateAttributesJobified();
            Assert.AreEqual(110f, clientAttributes.Health.CurrentValue, "Client health should be 110 with 1 stack");

            // 3. Add second application on server (increments stack count)
            var serverEffect2 = _testEffectDef.ToEffect(ServerSystem, ServerSystem);
            ServerSystem.EffectManager.AddEffectFromServer(serverEffect2);
            
            // Verify client has updated stack count
            Assert.AreEqual(2, clientEffect.NumStacks, "Client effect stack count should synchronize to 2");
            
            // Client attributes should have recalculation run with 2 stacks (+20 health)
            ClientSystem.AttributeSetManager.UpdateAttributesJobified();
            Assert.AreEqual(120f, clientAttributes.Health.CurrentValue, "Client health should be 120 with 2 stacks");
        }

        [Test]
        public void ServerDecreasesStackOfEffect_ClientReplicatesStackCountAndAttribute()
        {
            // Add TestAttributeSet to both server and client to verify attribute modifications
            var serverAttributes = new TestAttributeSet(ServerSystem);
            ServerSystem.AttributeSetManager.AddAttributeSet(typeof(TestAttributeSet), serverAttributes);
            var clientAttributes = new TestAttributeSet(ClientSystem);
            ClientSystem.AttributeSetManager.AddAttributeSet(typeof(TestAttributeSet), clientAttributes);

            // 1. Configure test effect to allow stacking, expiration policy to remove single stack, and duration to 5s
            _testEffectDef.DurationType = EffectDurationType.FixedDuration;
            _testEffectDef.DurationSeconds = 5f;
            _testEffectDef.EffectStack = new EffectStack 
            { 
                EffectStackType = EffectStackType.AggregateByTarget,
                MaxStacks = 3,
                EffectStackExpirationPolicy = EffectStackExpirationPolicy.RemoveSingleStackAndRefreshDuration,
                EffectStackDurationPolicy = EffectStackDurationPolicy.RefreshOnNewApplication
            };
            
            var floatModifier = new FloatModifier
            {
                AttributeName = "TestAttributeSet.Health",
                Operation = EffectOperation.Additive,
                ModifierMagnitude = 10f
            };
            _testEffectDef.Modifiers = new Modifier[] { floatModifier };

            // Start with a mock manual time of 0
            var serverNetwork = (MockNetworkRole)ServerSystem.NetworkRole;
            var clientNetwork = (MockNetworkRole)ClientSystem.NetworkRole;
            serverNetwork.ManualTime = 0;
            clientNetwork.ManualTime = 0;

            // 2. Add first application on server
            var serverEffect = _testEffectDef.ToEffect(ServerSystem, ServerSystem);
            ServerSystem.EffectManager.AddEffectFromServer(serverEffect);
            
            // 3. Add second application on server (increments stack count)
            var serverEffect2 = _testEffectDef.ToEffect(ServerSystem, ServerSystem);
            ServerSystem.EffectManager.AddEffectFromServer(serverEffect2);
            
            // Verify client has updated stack count
            var clientEffect = ClientSystem.EffectManager.Effects.First(e => e.Definition.name == "TestEffect");
            Assert.AreEqual(2, clientEffect.NumStacks, "Client effect stack count should synchronize to 2");
            
            // Client attributes should have recalculation run with 2 stacks (+20 health)
            ClientSystem.AttributeSetManager.UpdateAttributesJobified();
            Assert.AreEqual(120f, clientAttributes.Health.CurrentValue, "Client health should be 120 with 2 stacks");

            // 4. Advance time by 6 seconds (beyond 5s duration)
            serverNetwork.ManualTime = 6;
            clientNetwork.ManualTime = 6;

            // 5. Tick server to expire the first stack
            ServerSystem.Tick();

            // Verify client has updated stack count (decreased to 1)
            Assert.AreEqual(1, clientEffect.NumStacks, "Client effect stack count should synchronize to 1 after server stack expiration");
            
            // Client attributes should have recalculation run with 1 stack (+10 health)
            ClientSystem.AttributeSetManager.UpdateAttributesJobified();
            Assert.AreEqual(110f, clientAttributes.Health.CurrentValue, "Client health should be 110 after stack decreased to 1");
        }
    }
}
