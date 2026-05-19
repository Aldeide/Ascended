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
    /// <summary>
    /// Tests for the Reload and Fire ability logic, focusing on ammo consumption, 
    /// automatic reloading, and network prediction rollback for weapon attributes.
    /// </summary>
    public class ReloadTests : AbilitySystemTestBase
    {
        protected override bool AddDefaultAttributes => false;
        
        private WeaponAttributeSet _clientWeaponSet;
        private CharacteristicsAttributeSet _clientCharSet;
        
        private WeaponAttributeSet _serverWeaponSet;
        private CharacteristicsAttributeSet _serverCharSet;

        private FireAbilityDefinition _fireAbilityDef;
        private ReloadWeaponAbilityDefinition _reloadAbilityDef;
        private EffectDefinition _reloadCostDef;
        private ReloadCostMMC _reloadMMC;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            // Setup roles
            SourceMock.Setup(x => x.IsServer()).Returns(false);
            SourceMock.Setup(x => x.IsLocalClient()).Returns(true);
            TargetMock.Setup(x => x.IsServer()).Returns(true);
            TargetMock.Setup(x => x.IsLocalClient()).Returns(false);

            // Add extension attribute sets
            _clientWeaponSet = new WeaponAttributeSet(Source);
            _clientCharSet = new CharacteristicsAttributeSet(Source);
            Source.AttributeSetManager.AddAttributeSet(typeof(WeaponAttributeSet), _clientWeaponSet);
            Source.AttributeSetManager.AddAttributeSet(typeof(CharacteristicsAttributeSet), _clientCharSet);

            _serverWeaponSet = new WeaponAttributeSet(Target);
            _serverCharSet = new CharacteristicsAttributeSet(Target);
            Target.AttributeSetManager.AddAttributeSet(typeof(WeaponAttributeSet), _serverWeaponSet);
            Target.AttributeSetManager.AddAttributeSet(typeof(CharacteristicsAttributeSet), _serverCharSet);

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
            Source.AbilityManager.GrantAbility(_fireAbilityDef);
            Source.AbilityManager.GrantAbility(_reloadAbilityDef);
            Target.AbilityManager.GrantAbility(_fireAbilityDef);
            Target.AbilityManager.GrantAbility(_reloadAbilityDef);
        }

        /// <summary>
        /// Validates that activating the Fire ability correctly consumes one unit of ammunition.
        /// </summary>
        [Test]
        public void ReloadTests_FireAbility_ConsumesAmmo()
        {
            AbilitySystemUtilities.LinkAbilitySystems(SourceMock, TargetMock);
            _clientWeaponSet.CurrentClip.SetBaseValue(30);
            _serverWeaponSet.CurrentClip.SetBaseValue(30);
            
            bool success = Source.AbilityManager.TryActivateAbility("FireAbility");
            Assert.IsTrue(success);
            
            Assert.AreEqual(29, _clientWeaponSet.CurrentClip.CurrentValue);
        }

        /// <summary>
        /// Validates that the Fire ability automatically triggers the Reload ability when ammunition is empty.
        /// </summary>
        [Test]
        public void ReloadTests_FireAbility_TriggersReloadWhenAmmoEmpty()
        {
            AbilitySystemUtilities.LinkAbilitySystems(SourceMock, TargetMock);
            _clientCharSet.Energy.SetBaseValue(100);
            _serverCharSet.Energy.SetBaseValue(100);
            _clientWeaponSet.CurrentClip.SetBaseValue(1);
            _serverWeaponSet.CurrentClip.SetBaseValue(1);
            
            bool success = Source.AbilityManager.TryActivateAbility("FireAbility");
            Assert.IsTrue(success, "FireAbility should activate");
            
            // Should have triggered reload
            Assert.IsTrue(Source.AbilityManager.Abilities["ReloadWeaponAbility"].IsActive);
        }

        /// <summary>
        /// Validates that the Reload Magnitude Calculation (MMC) correctly calculates energy cost based on missing ammo.
        /// </summary>
        [Test]
        public void ReloadTests_ReloadMMC_CalculatesCorrectCost()
        {
            _clientWeaponSet.CurrentClip.SetBaseValue(50); // 50 missing
            _clientWeaponSet.ClipSize.SetBaseValue(100);
            _clientWeaponSet.ReloadEnergyCost.SetBaseValue(10);
            
            var effect = new Effect(new EffectDefinition());
            effect.Initialise(Source, Source);
            var cost = _reloadMMC.CalculateMagnitude(effect, 1.0f);
            
            // (50/100) * 10 = 5
            Assert.AreEqual(5f, cost);
        }

        /// <summary>
        /// Validates that if the server denies a Reload activation (e.g. lack of energy), 
        /// the client correctly rolls back its predicted energy consumption and ammo state.
        /// </summary>
        [Test]
        public void ReloadTests_ReloadPrediction_RollsBackWhenServerDenies()
        {
            // Setup: Client thinks it has enough energy, server knows it doesn't
            _clientCharSet.Energy.SetBaseValue(100);
            _serverCharSet.Energy.SetBaseValue(0); 

            _clientWeaponSet.CurrentClip.SetBaseValue(0);
            _serverWeaponSet.CurrentClip.SetBaseValue(0);
            
            _clientWeaponSet.ReloadEnergyCost.SetBaseValue(50);
            _serverWeaponSet.ReloadEnergyCost.SetBaseValue(50);

            PredictionKey capturedKey = default;
            bool serverSuccess = false;

            var mockRep = (MockReplicationManager)Source.ReplicationManager;
            mockRep.OnServerAbilityActivationRequested += (name, key, data) =>
            {
                capturedKey = key;
                serverSuccess = Target.AbilityManager.TryActivateAbility(name, data);
            };

            // Client activates reload
            bool success = Source.AbilityManager.TryActivateAbility("ReloadWeaponAbility");
            Assert.IsTrue(success, "Client should believe it can activate");
            Assert.IsTrue(Source.AbilityManager.Abilities["ReloadWeaponAbility"].IsActive);
            
            // Energy should have been consumed on client (predicted)
            Assert.Less(_clientCharSet.Energy.CurrentValue, 100);

            // Now manually trigger the server response on the client
            Source.AbilityManager.NotifyServerResponse(capturedKey, serverSuccess);
            
            Assert.IsFalse(serverSuccess, "Server should have denied activation");
            Assert.IsFalse(Source.AbilityManager.Abilities["ReloadWeaponAbility"].IsActive, "Ability should have been rolled back");
            Assert.AreEqual(100, _clientCharSet.Energy.CurrentValue, "Energy should have been rolled back");
            Assert.AreEqual(0, _clientWeaponSet.CurrentClip.CurrentValue, "Ammo should have been rolled back");
        }

        /// <summary>
        /// Validates that if the server denies a Fire activation (e.g. out of sync ammo count),
        /// the client correctly rolls back its predicted ammo consumption.
        /// </summary>
        [Test]
        public void ReloadTests_FirePrediction_RollsBackWhenServerDenies()
        {
            // Setup: Client thinks it has ammo, server knows it doesn't
            _clientWeaponSet.CurrentClip.SetBaseValue(1);
            _serverWeaponSet.CurrentClip.SetBaseValue(0);

            PredictionKey capturedKey = default;
            bool serverSuccess = false;

            var mockRep = (MockReplicationManager)Source.ReplicationManager;
            mockRep.OnServerAbilityActivationRequested += (name, key, data) =>
            {
                capturedKey = key;
                serverSuccess = Target.AbilityManager.TryActivateAbility(name, data);
            };

            // Client activates fire
            bool success = Source.AbilityManager.TryActivateAbility("FireAbility");
            Assert.IsTrue(success, "Client should predict fire success");
            Assert.AreEqual(0, _clientWeaponSet.CurrentClip.CurrentValue, "Ammo should be predicted to 0");

            // Server response (deny)
            Source.AbilityManager.NotifyServerResponse(capturedKey, serverSuccess);

            Assert.IsFalse(serverSuccess, "Server should have denied fire");
            Assert.AreEqual(1, _clientWeaponSet.CurrentClip.CurrentValue, "Ammo should have been rolled back to 1");
        }
    }
}
