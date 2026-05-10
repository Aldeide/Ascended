using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.Cooldowns;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Networking
{
    [TestFixture]
    public class ChargesAbilitySyncTests : AbilitySystemSyncTestBase
    {
        private ChargesAbilityDefinition _abilityDef;
        private EffectDefinition _cooldownEffectDef;
        private Tag _cooldownTag;

        public override void Setup()
        {
            base.Setup();

            // Setup Cooldown Effect
            _cooldownTag = new Tag("Ability.Cooldown.Test");
            _cooldownEffectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            _cooldownEffectDef.name = "TestCooldownEffect";
            _cooldownEffectDef.DurationSeconds = 1.0f;
            _cooldownEffectDef.DurationType = EffectDurationType.FixedDuration;
            _cooldownEffectDef.GrantedTags = new[] { _cooldownTag };

            // Setup Ability
            _abilityDef = ScriptableObject.CreateInstance<ChargesAbilityDefinition>();
            _abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            _abilityDef.UniqueName = "TestChargesAbility";
            _abilityDef.MaxCharges = 3;
            
            // Setup Cooldown in Definition
            var cooldown = new ConstantAbilityCooldown() { CooldownEffect = _cooldownEffectDef };
            _abilityDef.Cooldown = cooldown;

            // Register in Mock DataManager
            DataManager.Abilities[_abilityDef.UniqueName] = _abilityDef;
            DataManager.Effects[_cooldownEffectDef.name] = _cooldownEffectDef;

            // Grant to both systems
            ServerSystem.AbilityManager.GrantAbility(_abilityDef);
            ClientSystem.AbilityManager.GrantAbility(_abilityDef);
        }

        [Test]
        public void ChargesAbilitySyncTests_OwnerActivation_ChargesDropLocallyAndSync()
        {
            var clientAbility = (ChargesAbility)ClientSystem.AbilityManager.Abilities[_abilityDef.UniqueName];
            int chargesChangedCount = 0;
            clientAbility.OnChargesChanged += (curr, max) => chargesChangedCount++;

            // 1. Client attempts activation (predicted)
            bool activated = ClientSystem.AbilityManager.TryActivateAbility(_abilityDef.UniqueName);

            Assert.IsTrue(activated, "Ability should activate on client");
            Assert.AreEqual(2, clientAbility.CurrentCharges, "Charges should drop locally on client");
            Assert.AreEqual(2, chargesChangedCount, "OnChargesChanged should fire twice (local drop + server sync)");

            // 2. Server processes activation
            var serverAbility = (ChargesAbility)ServerSystem.AbilityManager.Abilities[_abilityDef.UniqueName];
            Assert.AreEqual(2, serverAbility.CurrentCharges, "Server charges should match client");
        }

        [Test]
        public void ChargesAbilitySyncTests_ChargeRegeneration_SyncsFromServerToClient()
        {
            var clientAbility = (ChargesAbility)ClientSystem.AbilityManager.Abilities[_abilityDef.UniqueName];
            var serverAbility = (ChargesAbility)ServerSystem.AbilityManager.Abilities[_abilityDef.UniqueName];

            // 1. Drop a charge
            ClientSystem.AbilityManager.TryActivateAbility(_abilityDef.UniqueName);
            Assert.AreEqual(2, serverAbility.CurrentCharges);
            Assert.AreEqual(2, clientAbility.CurrentCharges);

            // 2. Cooldown should be active on server
            ServerSystem.Tick();
            ClientSystem.Tick();
            Debug.Log("Server Effects: " + ServerSystem.EffectManager.DebugString());
            Debug.Log("Client Effects: " + ClientSystem.EffectManager.DebugString());
            Assert.IsTrue(ServerSystem.TagManager.HasTag(_cooldownTag), "Server should have cooldown tag");
            Assert.IsTrue(ClientSystem.TagManager.HasTag(_cooldownTag), "Client should have replicated cooldown tag");

            // 3. Manually expire cooldown on server
            var cooldownEffect = ServerSystem.EffectManager.Effects.First(e => e.Definition == _cooldownEffectDef);
            ServerSystem.EffectManager.RemoveEffect(cooldownEffect);
            
            // 4. Server Tick should trigger recharge
            ServerSystem.Tick();
            Assert.AreEqual(3, serverAbility.CurrentCharges, "Server should have recharged");

            // 5. Client Tick should see the transition and recharge
            ClientSystem.Tick();
            Assert.AreEqual(3, clientAbility.CurrentCharges, "Client should have recharged following server removal");
        }

        [Test]
        public void ChargesAbilitySyncTests_LateJoiner_SyncsInitialCharges()
        {
            // 1. Server drops charges
            var serverAbility = (ChargesAbility)ServerSystem.AbilityManager.Abilities[_abilityDef.UniqueName];
            ServerSystem.AbilityManager.TryActivateAbility(_abilityDef.UniqueName); // 3 -> 2
            ServerSystem.AbilityManager.TryActivateAbility(_abilityDef.UniqueName); // 2 -> 1
            Assert.AreEqual(1, serverAbility.CurrentCharges);

            // 2. Simulate join sync on existing client
            SyncAbilities();
            
            var clientAbility = (ChargesAbility)ClientSystem.AbilityManager.Abilities[_abilityDef.UniqueName];
            Assert.AreEqual(1, clientAbility.CurrentCharges, "Client should sync to server charges");
        }
        
        // We need to decide what is replicated to observers.
        [Test]
        public void ChargesAbilitySyncTests_Observer_SeesOtherPlayerActivation()
        {
            // Setup a second client (Observer) - must set DataManager so ProcessClientEffectAdded can resolve definitions
            var observerSystem = new AbilitySystemManager(DataManager);
            observerSystem.NetworkRole = new MockNetworkRole(false, false, 4);
            observerSystem.ReplicationManager.DataManager = DataManager;
            observerSystem.AbilityManager.GrantAbility(_abilityDef);
            var observerAbility = (ChargesAbility)observerSystem.AbilityManager.Abilities[_abilityDef.UniqueName];

            // Link server to observer (broad replication)
            ServerSystem.ReplicationManager.OnNotifyClientsEffectAdded += (data) => observerSystem.ReplicationManager.ProcessClientEffectAdded(data);
            ServerSystem.ReplicationManager.OnNotifyClientsEffectRemoved += (name) => observerSystem.ReplicationManager.ProcessClientEffectRemoved(name);
            ServerSystem.ReplicationManager.OnClientActivateAbility += (name, data) => observerSystem.AbilityManager.ForceActivateAbility(name, data);
            ServerSystem.ReplicationManager.OnNotifyClientsAbilityChargesChanged += (name, curr, max) => observerSystem.ReplicationManager.ProcessClientAbilityChargesChanged(name, curr, max);

            // 1. Client (Owner) activates (predicted) - server processes synchronously in test
            ClientSystem.AbilityManager.TryActivateAbility(_abilityDef.UniqueName);

            // 2. Observer should immediately see charges drop via broadcast
            Assert.AreEqual(2, observerAbility.CurrentCharges, "Observer should see charge drop via broadcast");

            // 3. Tick the server so ChargesAbility starts the cooldown effect (cooldown is deferred to Tick)
            ServerSystem.Tick();

            // 4. Observer should now see the cooldown tag replicated
            Assert.IsTrue(observerSystem.TagManager.HasTag(_cooldownTag), "Observer should see cooldown start after server Tick");
        }
        [Test]
        public void ChargesAbilitySyncTests_CooldownUIEvents_FireOnClient()
        {
            var clientAbility = (ChargesAbility)ClientSystem.AbilityManager.Abilities[_abilityDef.UniqueName];
            var cooldownStarted = false;
            var progress = 0f;
            
            clientAbility.OnCooldownStarted += (duration) => cooldownStarted = true;
            clientAbility.OnCooldownProgressChanged += (p) => progress = p;

            // 1. Client triggers ability
            ClientSystem.AbilityManager.TryActivateAbility(_abilityDef.UniqueName);
            ClientSystem.Tick();
            ServerSystem.Tick();
            
            // 2. Client receives effect and tags
            Assert.IsTrue(ClientSystem.TagManager.HasTag(_cooldownTag));

            // 3. Advance time and Client Tick should update progress
            var clientRole = (MockNetworkRole)ClientSystem.NetworkRole;
            clientRole.ManualTime += 0.5f;
            ClientSystem.Tick();
            
            Assert.IsTrue(cooldownStarted, "OnCooldownStarted should fire on client when server cooldown replicates");
            Assert.AreEqual(0.5f, progress, 0.01f, "Cooldown progress should be calculated correctly on client after time advance");
        }

        private class CooldownImplementation : AbilityCooldown
        {
            public override float Calculate(IAbilitySystem owner) => 1.0f;
        }
    }
}
