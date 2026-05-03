using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Networking
{
    /// <summary>
    /// Unit tests for network reconciliation, verifying that client-side predictions are correctly confirmed, denied, or rolled back by the server.
    /// </summary>
    public class ReconciliationTests : AbilitySystemTestBase
    {
        private MockReplicationManager _serverRep;
        private MockReplicationManager _clientRep;

        [SetUp]
        public override void SetUp()
        {
            // Set up a server and a client system for interaction testing
            SourceMock = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            TargetMock = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            
            _clientRep = (MockReplicationManager)Source.ReplicationManager;
            _serverRep = (MockReplicationManager)Target.ReplicationManager;

            // Link them so client requests go to server, and server responses go to client
            _clientRep.OnServerAbilityActivationRequested = (name, key, data) => 
                _serverRep.ProcessServerAbilityActivation(name, key, data);
            
            _serverRep.OnAbilityActivationResponded = (key, success) => 
            {
                if (success) Source.ReplicationManager.ProcessAbilityActivationConfirmed(key);
                else Source.ReplicationManager.ProcessAbilityActivationDenied("Unknown", key); // Simplified for base setup
            };

            base.SetUp();
        }

        /// <summary>
        /// Verifies that a successfully predicted ability remains active on the client after receiving server confirmation.
        /// </summary>
        [Test]
        public void ReconciliationTests_SuccessfulPrediction_ServerConfirmsAndClientStaysActive()
        {
            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.Success";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            
            Source.AbilityManager.GrantAbility(abilityDef);
            Target.AbilityManager.GrantAbility(abilityDef);

            // Trigger prediction
            Source.AbilityManager.TryActivateAbility("Test.Ability.Success");

            Assert.IsTrue(Target.AbilityManager.Abilities["Test.Ability.Success"].IsActive, "Server ability should have activated");
            Assert.IsTrue(Source.AbilityManager.Abilities["Test.Ability.Success"].IsActive, "Client ability should stay active after confirmation");
        }

        /// <summary>
        /// Verifies that when the server denies a predicted ability, the client correctly terminates the predicted instance.
        /// </summary>
        [Test]
        public void ReconciliationTests_ServerDenial_ClientRollsBackPredictedAbility()
        {
            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.Denied";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            
            Source.AbilityManager.GrantAbility(abilityDef);
            // Specifically DO NOT grant on server to cause denial

            // Overwrite response to include the specific name for rollback logic
            _serverRep.OnAbilityActivationResponded = (key, success) => 
                Source.ReplicationManager.ProcessAbilityActivationDenied("Test.Ability.Denied", key);

            Source.AbilityManager.TryActivateAbility("Test.Ability.Denied");

            Assert.IsFalse(Source.AbilityManager.Abilities["Test.Ability.Denied"].IsActive, "Client ability should have rolled back after server denial");
        }

        /// <summary>
        /// Verifies that during reconciliation, a predicted effect is correctly replaced by its authoritative server version without resulting in duplicates.
        /// </summary>
        [Test]
        public void ReconciliationTests_EffectReconciliation_PreventsDuplicateEffects()
        {
            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.Effects";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            
            var effectDef = EffectUtilities.CreateDurationEffectDefinition();
            abilityDef.GrantedEffects = new[] { effectDef };

            Source.AbilityManager.GrantAbility(abilityDef);
            Target.AbilityManager.GrantAbility(abilityDef);

            // Intercept server activation to simulate sending back the authoritative effect
            _clientRep.OnServerAbilityActivationRequested = (name, key, data) =>
            {
                _serverRep.ProcessServerAbilityActivation(name, key, data);
                
                var serverEffect = effectDef.ToEffect(Target, Source);
                serverEffect.PredictionKey = key;
                
                Source.ReplicationManager.ProcessAbilityActivationConfirmed(key);
                Source.EffectManager.ReconcilePredictedEffect(key, serverEffect);
            };

            Source.AbilityManager.TryActivateAbility("Test.Ability.Effects");

            var activeEffects = Source.EffectManager.GetActiveEffects();
            Assert.AreEqual(1, activeEffects.Count, "Client should have exactly one authoritative effect; predicted should have been replaced");
        }

        /// <summary>
        /// Verifies that attribute changes caused by a predicted cost are correctly rolled back if the server denies the ability.
        /// </summary>
        [Test]
        public void ReconciliationTests_AttributeRollback_RestoresValueOnServerDenial()
        {
            var abilityDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDef.UniqueName = "Test.Ability.Cost";
            abilityDef.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            
            var costDef = ScriptableObject.CreateInstance<EffectDefinition>();
            costDef.DurationType = EffectDurationType.Instant;
            costDef.Modifiers = new Modifier[] {
                new FloatModifier {
                    AttributeName = "TestAttributeSet.Health",
                    Operation = EffectOperation.Subtractive,
                    ModifierMagnitude = 10f
                }
            };
            abilityDef.Cost = costDef;

            Source.AbilityManager.GrantAbility(abilityDef);
            
            // Initial health from SourceAttributes (TestAttributeSet)
            float initialHealth = SourceAttributes.Health.CurrentValue;

            // Server denies
            _clientRep.OnServerAbilityActivationRequested = (name, key, data) =>
                Source.ReplicationManager.ProcessAbilityActivationDenied(name, key);

            Source.AbilityManager.TryActivateAbility("Test.Ability.Cost");

            Assert.AreEqual(initialHealth, SourceAttributes.Health.CurrentValue, "Health should have been restored to its initial value after predicted cost was rolled back");
        }
    }
}
