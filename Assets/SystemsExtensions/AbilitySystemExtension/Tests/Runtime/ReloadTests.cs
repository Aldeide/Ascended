using System.Collections;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Test.Utilities;
using AbilitySystemExtension.Runtime.Abilities;
using AbilitySystemExtension.Runtime.AttributeSets;
using AbilitySystemExtension.Runtime.Calculations;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AbilitySystemExtension.Tests.Runtime
{
    public class ReloadTests
    {
        private Mock<IAbilitySystem> _client;
        private Mock<IAbilitySystem> _server;
        
        private WeaponAttributeSet _clientWeaponSet;
        private CharacteristicsAttributeSet _clientCharSet;
        
        private WeaponAttributeSet _serverWeaponSet;
        private CharacteristicsAttributeSet _serverCharSet;

        private FireAbilityDefinition _fireAbilityDef;
        private ReloadWeaponAbilityDefinition _reloadAbilityDef;
        private EffectDefinition _reloadCostDef;
        private ReloadCostMMC _reloadMMC;

        [SetUp]
        public void SetUp()
        {
            _client = CreateCustomMockAbilitySystem(false);
            _server = CreateCustomMockAbilitySystem(true);

            _clientWeaponSet = new WeaponAttributeSet(_client.Object);
            _clientCharSet = new CharacteristicsAttributeSet(_client.Object);
            _client.Object.AttributeSetManager.AddAttributeSet(typeof(WeaponAttributeSet), _clientWeaponSet);
            _client.Object.AttributeSetManager.AddAttributeSet(typeof(CharacteristicsAttributeSet), _clientCharSet);

            _serverWeaponSet = new WeaponAttributeSet(_server.Object);
            _serverCharSet = new CharacteristicsAttributeSet(_server.Object);
            _server.Object.AttributeSetManager.AddAttributeSet(typeof(WeaponAttributeSet), _serverWeaponSet);
            _server.Object.AttributeSetManager.AddAttributeSet(typeof(CharacteristicsAttributeSet), _serverCharSet);

            // Initialize Definition mocks
            _fireAbilityDef = ScriptableObject.CreateInstance<FireAbilityDefinition>();
            _fireAbilityDef.UniqueName = "FireAbility";
            _fireAbilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;

            _reloadAbilityDef = ScriptableObject.CreateInstance<ReloadWeaponAbilityDefinition>();
            _reloadAbilityDef.UniqueName = "ReloadWeaponAbility";
            _reloadAbilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;

            _reloadCostDef = ScriptableObject.CreateInstance<EffectDefinition>();
            _reloadCostDef.DurationType = EffectDurationType.Instant;
            
            _reloadMMC = ScriptableObject.CreateInstance<ReloadCostMMC>();
            
            var costModifier = new CalculationModifier();
            costModifier.AttributeName = "CharacteristicsAttributeSet.Energy";
            costModifier.Operation = EffectOperation.Subtractive;
            costModifier.calculation = _reloadMMC;
            
            _reloadCostDef.Modifiers = new Modifier[] { costModifier };
            _reloadAbilityDef.Cost = _reloadCostDef;

            // Grant abilities
            _client.Object.AbilityManager.GrantAbility(_fireAbilityDef);
            _client.Object.AbilityManager.GrantAbility(_reloadAbilityDef);
            _server.Object.AbilityManager.GrantAbility(_fireAbilityDef);
            _server.Object.AbilityManager.GrantAbility(_reloadAbilityDef);
        }

        private Mock<IAbilitySystem> CreateCustomMockAbilitySystem(bool isServer)
        {
            var owner = new Mock<IAbilitySystem>();
            owner.Setup(x => x.IsServer()).Returns(isServer);
            owner.Setup(x => x.IsLocalClient()).Returns(!isServer);
            owner.Setup(x => x.IsHost()).Returns(false);

            var networkRole = new Mock<INetworkRole>();
            networkRole.SetupGet(nr => nr.NetworkObjectId).Returns(1);
            owner.SetupGet(o => o.NetworkRole).Returns(networkRole.Object);

            var effectManager = new EffectManager(owner.Object);
            owner.Setup(x => x.EffectManager).Returns(effectManager);
            var eventManager = new EventManager();
            owner.Setup(x => x.EventManager).Returns(eventManager);
            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(x => x.TagManager).Returns(tagManager);
            
            // This is the key: Don't add TestAttributeSet here!
            var attributeSetManager = new AttributeSetManager(owner.Object);
            owner.SetupGet(x => x.AttributeSetManager).Returns(attributeSetManager);
            
            var replicationManager = new MockReplicationManager(owner.Object);
            owner.Setup(x => x.ReplicationManager).Returns(replicationManager);
            var abilityManager = new AbilityManager(owner.Object);
            owner.Setup(x => x.AbilityManager).Returns(abilityManager);
            var dataManager = new Mock<IDataManager>();
            owner.Setup(x => x.DataManager).Returns(dataManager.Object);
            replicationManager.DataManager = dataManager.Object;

            owner.Setup(x => x.MakeEffectContext()).Returns(() => new EffectContext(owner.Object, owner.Object));
            owner.Setup(x => x.MakeOutgoingEffect(It.IsAny<EffectDefinition>(), It.IsAny<int>(), It.IsAny<EffectContext>()))
                .Returns((EffectDefinition def, int level, EffectContext context) => def.ToEffect(owner.Object, owner.Object, context));
            owner.Setup(x => x.ApplyEffectToSelf(It.IsAny<Effect>()))
                .Returns((Effect eff) =>
                {
                    eff.Initialise(eff.Source, owner.Object, eff.Context, eff.Level);
                    eff.Activate();
                    return owner.Object.EffectManager.AddEffect(eff);
                });

            owner.Setup(x => x.GetGameObjectFromNetworkId(It.IsAny<ulong>()))
                .Returns((ulong id) => null);

            return owner;
        }

        [Test]
        public void FireAbility_ConsumesAmmo()
        {
            AbilitySystemUtilities.LinkAbilitySystems(_client, _server);
            _clientWeaponSet.CurrentClip.SetCurrentValue(30);
            
            bool success = _client.Object.AbilityManager.TryActivateAbility("FireAbility");
            Assert.IsTrue(success);
            
            Assert.AreEqual(29, _clientWeaponSet.CurrentClip.CurrentValue);
        }

        [Test]
        public void FireAbility_TriggersReload_WhenAmmoEmpty()
        {
            AbilitySystemUtilities.LinkAbilitySystems(_client, _server);
            _clientCharSet.Energy.SetCurrentValue(100);
            _clientWeaponSet.CurrentClip.SetCurrentValue(1);
            
            bool success = _client.Object.AbilityManager.TryActivateAbility("FireAbility");
            Assert.IsTrue(success, "FireAbility should activate");
            
            // Should have triggered reload
            Assert.IsTrue(_client.Object.AbilityManager.Abilities["ReloadWeaponAbility"].IsActive);
        }

        [Test]
        public void ReloadMMC_CalculatesCorrectCost()
        {
            // No link needed
            _clientWeaponSet.CurrentClip.SetCurrentValue(50); // 50 missing
            _clientWeaponSet.ClipSize.SetBaseValue(100);
            _clientWeaponSet.ReloadEnergyCost.SetBaseValue(10);
            
            // Need a dummy effect for the MMC
            var effect = new Effect(new EffectDefinition());
            effect.Initialise(_client.Object, _client.Object);
            var cost = _reloadMMC.CalculateMagnitude(effect, 1.0f);
            
            // (50/100) * 10 = 5
            Assert.AreEqual(5f, cost);
        }

        [Test]
        public void ReloadPrediction_RollsBack_WhenServerDenies()
        {
            // Setup: Client thinks it has enough energy, server knows it doesn't
            _clientCharSet.Energy.SetCurrentValue(100);
            _serverCharSet.Energy.SetCurrentValue(0); 

            _clientWeaponSet.CurrentClip.SetCurrentValue(0);
            _serverWeaponSet.CurrentClip.SetCurrentValue(0);
            
            _clientWeaponSet.ReloadEnergyCost.SetBaseValue(50);
            _serverWeaponSet.ReloadEnergyCost.SetBaseValue(50);

            // We'll manually handle the server response to avoid synchronous race condition
            PredictionKey capturedKey = default;
            bool serverSuccess = false;

            var mockRep = (MockReplicationManager)_client.Object.ReplicationManager;
            mockRep.OnServerAbilityActivationRequested += (name, key, data) =>
            {
                capturedKey = key;
                serverSuccess = _server.Object.AbilityManager.TryActivateAbility(name, data);
            };

            // Client activates reload
            bool success = _client.Object.AbilityManager.TryActivateAbility("ReloadWeaponAbility");
            Assert.IsTrue(success, "Client should believe it can activate");
            Assert.IsTrue(_client.Object.AbilityManager.Abilities["ReloadWeaponAbility"].IsActive);
            
            // Energy should have been consumed on client (predicted)
            Assert.Less(_clientCharSet.Energy.CurrentValue, 100);

            // Now manually trigger the server response on the client
            _client.Object.AbilityManager.NotifyServerResponse(capturedKey, serverSuccess);
            
            Assert.IsFalse(serverSuccess, "Server should have denied activation");
            Assert.IsFalse(_client.Object.AbilityManager.Abilities["ReloadWeaponAbility"].IsActive, "Ability should have been rolled back");
            Assert.AreEqual(100, _clientCharSet.Energy.CurrentValue, "Energy should have been rolled back");
            Assert.AreEqual(0, _clientWeaponSet.CurrentClip.CurrentValue, "Ammo should have been rolled back");
        }

        [Test]
        public void FirePrediction_RollsBack_WhenServerDenies()
        {
            // Setup: Client thinks it has ammo, server knows it doesn't
            _clientWeaponSet.CurrentClip.SetCurrentValue(1);
            _serverWeaponSet.CurrentClip.SetCurrentValue(0);

            // Manual network response
            PredictionKey capturedKey = default;
            bool serverSuccess = false;

            var mockRep = (MockReplicationManager)_client.Object.ReplicationManager;
            mockRep.OnServerAbilityActivationRequested += (name, key, data) =>
            {
                capturedKey = key;
                serverSuccess = _server.Object.AbilityManager.TryActivateAbility(name, data);
            };

            // Client activates fire
            bool success = _client.Object.AbilityManager.TryActivateAbility("FireAbility");
            Assert.IsTrue(success, "Client should predict fire success");
            Assert.AreEqual(0, _clientWeaponSet.CurrentClip.CurrentValue, "Ammo should be predicted to 0");

            // Server response (deny)
            _client.Object.AbilityManager.NotifyServerResponse(capturedKey, serverSuccess);

            Assert.IsFalse(serverSuccess, "Server should have denied fire");
            Assert.AreEqual(1, _clientWeaponSet.CurrentClip.CurrentValue, "Ammo should have been rolled back to 1");
        }
    }
}
