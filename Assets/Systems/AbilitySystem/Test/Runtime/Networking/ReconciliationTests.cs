using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using Moq;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using AbilitySystem.Runtime.Effects;
using System.Linq;
using AbilitySystem.Runtime.Modifiers;

namespace AbilitySystem.Test.Runtime.Networking
{
    public class ReconciliationTests
    {
        [Test]
        public void Reconciliation_SuccessfulPrediction_ServerConfirms()
        {
            // Arrange
            var serverSystem = CreateMockServerAbilitySystem();
            var clientSystem = CreateMockClientAbilitySystem();

            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.Success";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            
            clientSystem.Object.AbilityManager.GrantAbility(abilityDef);
            serverSystem.Object.AbilityManager.GrantAbility(abilityDef);

            clientSystem.Object.ReplicationManager.OnServerAbilityActivationRequested += (name, key, data) =>
            {
                serverSystem.Object.ReplicationManager.ProcessServerAbilityActivation(name, key, data);
            };

            serverSystem.Object.ReplicationManager.OnAbilityActivationResponded += (key, success) =>
            {
                if (success) clientSystem.Object.ReplicationManager.ProcessAbilityActivationConfirmed(key);
                else clientSystem.Object.ReplicationManager.ProcessAbilityActivationDenied(abilityDef.UniqueName, key);
            };

            // Act
            clientSystem.Object.AbilityManager.TryActivateAbility("Test.Ability.Success");

            // Assert
            Assert.IsTrue(serverSystem.Object.AbilityManager.Abilities["Test.Ability.Success"].IsActive, "Server ability should be active.");
            Assert.IsTrue(clientSystem.Object.AbilityManager.Abilities["Test.Ability.Success"].IsActive, "Client ability should stay active (confirmed).");
        }

        [Test]
        public void Reconciliation_ServerDenial_ClientRollsBack()
        {
            // Arrange
            var serverSystem = CreateMockServerAbilitySystem();
            var clientSystem = CreateMockClientAbilitySystem();

            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.Denied";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            
            clientSystem.Object.AbilityManager.GrantAbility(abilityDef);
            // DO NOT GRANT ON SERVER (or otherwise make it fail)

            clientSystem.Object.ReplicationManager.OnServerAbilityActivationRequested += (name, key, data) =>
            {
                serverSystem.Object.ReplicationManager.ProcessServerAbilityActivation(name, key, data);
            };
            
            serverSystem.Object.ReplicationManager.OnAbilityActivationResponded += (key, success) =>
            {
                if (success) clientSystem.Object.ReplicationManager.ProcessAbilityActivationConfirmed(key);
                else clientSystem.Object.ReplicationManager.ProcessAbilityActivationDenied(abilityDef.UniqueName, key);
            };

            // Act
            clientSystem.Object.AbilityManager.TryActivateAbility("Test.Ability.Denied");

            // Assert
            Assert.IsFalse(clientSystem.Object.AbilityManager.Abilities["Test.Ability.Denied"].IsActive, "Client ability should have rolled back.");
        }

        [Test]
        public void Reconciliation_EffectReplacement_NoDuplicates()
        {
            // Arrange
            var serverSystem = CreateMockServerAbilitySystem();
            var clientSystem = CreateMockClientAbilitySystem();

            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.Effects";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            
            var effectDef = EffectUtilities.CreateDurationEffectDefinition();
            abilityDef.GrantedEffects = new[] { effectDef };

            clientSystem.Object.AbilityManager.GrantAbility(abilityDef);
            serverSystem.Object.AbilityManager.GrantAbility(abilityDef);

            clientSystem.Object.ReplicationManager.OnServerAbilityActivationRequested += (name, key, data) =>
            {
                // Server confirms 
                serverSystem.Object.ReplicationManager.ProcessServerAbilityActivation(name, key, data);
                
                // SIMULATE SERVER SENDING AUTHORITATIVE EFFECT
                var serverEffect = effectDef.ToEffect(serverSystem.Object, clientSystem.Object);
                serverEffect.IsActive = true; 
                serverEffect.PredictionKey = key;
                // Client receives response and reconcile (simulated RPC flow)
                clientSystem.Object.ReplicationManager.ProcessAbilityActivationConfirmed(key);
                clientSystem.Object.EffectManager.ReconcilePredictedEffect(key, serverEffect);
            };

            // Act
            clientSystem.Object.AbilityManager.TryActivateAbility("Test.Ability.Effects");

            // Assert
            var activeEffects = clientSystem.Object.EffectManager.GetActiveEffects();
            Assert.AreEqual(1, activeEffects.Count, "Should have exactly 1 active effect (predicted replaced by server).");
        }

        [Test]
        public void Reconciliation_AttributeRollback_OnDenial()
        {
            // Arrange
            var serverSystem = CreateMockServerAbilitySystem();
            var clientSystem = CreateMockClientAbilitySystem();

            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.Cost";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            
            // Define active cost
            var costDef = UnityEngine.ScriptableObject.CreateInstance<EffectDefinition>();
            costDef.DurationType = EffectDurationType.Instant;
            costDef.Modifiers = new Modifier[] {
                new FloatModifier {
                    AttributeName = "TestAttributeSet.Health",
                    Operation = EffectOperation.Subtractive,
                    ModifierMagnitude = 10f
                }
            };
            abilityDef.Cost = costDef;

            clientSystem.Object.AbilityManager.GrantAbility(abilityDef);
            // serverSystem stays empty to cause denial

            var health = clientSystem.Object.AttributeSetManager.GetAttribute("Health");
            float initialHealth = health.CurrentValue;

            clientSystem.Object.ReplicationManager.OnServerAbilityActivationRequested += (name, key, data) =>
            {
                // Server denies
                clientSystem.Object.ReplicationManager.ProcessAbilityActivationDenied(name, key);
            };

            // Act
            clientSystem.Object.AbilityManager.TryActivateAbility("Test.Ability.Cost");

            // Assert
            Assert.AreEqual(initialHealth, health.CurrentValue, "Health should have been restored after denial.");
        }
    }
}
