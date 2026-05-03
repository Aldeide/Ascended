using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Abilities
{
    /// <summary>
    /// Unit tests for general ability functionality, covering activation rules, tag interactions, and lifecycle management.
    /// </summary>
    public class AbilityTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Verifies that activating an ability correctly applies its associated attribute modifiers to the target.
        /// </summary>
        [Test]
        public void AbilityTests_ActivateAbility_AppliesAssociatedModifiers()
        {
            var abilityDefinition = AbilityUtilities.CreateInstantAbilityDefinition();
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            Source.AbilityManager.GrantAbility(abilityDefinition);
            
            bool success = Source.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            var healthAttribute = Source.AttributeSetManager.GetAttribute("Health");
            
            Assert.IsTrue(success);
            Assert.AreEqual(1000f, healthAttribute.CurrentValue);
            Assert.AreEqual(100f, healthAttribute.BaseValue);
        }
        
        /// <summary>
        /// Verifies that an ability fails to activate if the owner is missing its 'ActivationRequiredTags'.
        /// </summary>
        [Test]
        public void AbilityTests_MissingRequiredTags_ActivationFails()
        {
            var abilityDefinition = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDefinition.ActivationRequiredTags = new[] { new Tag("Tag.Missing") };
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            Source.AbilityManager.GrantAbility(abilityDefinition);
            
            bool success = Source.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsFalse(success, "Ability should fail activation when required tags are missing");
        }
        
        /// <summary>
        /// Verifies that an ability successfully activates when the owner possesses all 'ActivationRequiredTags'.
        /// </summary>
        [Test]
        public void AbilityTests_HasRequiredTags_ActivationSucceeds()
        {
            var abilityDefinition = AbilityUtilities.CreateInstantAbilityDefinition();
            var requiredTag = new Tag("Tag.Required");
            abilityDefinition.ActivationRequiredTags = new[] { requiredTag };
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            Source.TagManager.AddTag(requiredTag);
            Source.AbilityManager.GrantAbility(abilityDefinition);
            
            bool success = Source.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(success, "Ability should succeed activation when required tags are present");
        }
        
        /// <summary>
        /// Verifies that an ability fails to activate if the owner possesses any 'ActivationBlockedTags'.
        /// </summary>
        [Test]
        public void AbilityTests_HasBlockedTag_ActivationFails()
        {
            var abilityDefinition = AbilityUtilities.CreateInstantAbilityDefinition();
            var blockingTag = new Tag("Tag.Blocking");
            abilityDefinition.ActivationBlockedTags = new[] { blockingTag };
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            Source.TagManager.AddTag(blockingTag);
            Source.AbilityManager.GrantAbility(abilityDefinition);
            
            bool success = Source.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsFalse(success, "Ability should fail activation when blocking tags are present");
        }
        
        /// <summary>
        /// Verifies that activating an ability correctly cancels other active abilities that possess tags matching its 'CancelAbilityTags'.
        /// </summary>
        [Test]
        public void AbilityTests_CancelTags_CancelsMatchingActiveAbilities()
        {
            var cancelTag = new Tag("Ability.Type.Action");
            
            var abilityToCancelDef = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityToCancelDef.UniqueName = "Cancellable";
            abilityToCancelDef.AssetTags = new[] { cancelTag };
            
            var cancellerDef = AbilityUtilities.CreateInstantAbilityDefinition();
            cancellerDef.UniqueName = "Canceller";
            cancellerDef.CancelAbilityTags = new[] { cancelTag };
            
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            Source.AbilityManager.GrantAbility(abilityToCancelDef);
            Source.AbilityManager.GrantAbility(cancellerDef);
            
            Source.AbilityManager.TryActivateAbility("Cancellable");
            Assert.IsTrue(Source.AbilityManager.Abilities["Cancellable"].IsActive);
            
            Source.AbilityManager.TryActivateAbility("Canceller");
            
            Assert.IsFalse(Source.AbilityManager.Abilities["Cancellable"].IsActive, "Previous ability should have been cancelled");
        }
        
        /// <summary>
        /// Verifies that an ability correctly grants its 'ActivationOwnedTags' to the owner upon activation.
        /// </summary>
        [Test]
        public void AbilityTests_OwnedTags_GrantsTagsToOwnerUponActivation()
        {
            var ownedTag = new Tag("Ability.State.Active");
            var abilityDefinition = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDefinition.ActivationOwnedTags = new[] { ownedTag };
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            Source.AbilityManager.GrantAbility(abilityDefinition);
            
            Source.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(Source.TagManager.HasTag(ownedTag), "Owner should have received the owned tag");
        }
        
        /// <summary>
        /// Verifies that a client-only ability can be activated on the client without server permission.
        /// </summary>
        [Test]
        public void AbilityTests_ClientOnlyPolicy_ActivatesLocallyOnClient()
        {
            SourceMock.Setup(m => m.IsServer()).Returns(false);
            SourceMock.Setup(m => m.IsLocalClient()).Returns(true);
            
            var abilityDefinition = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            Source.AbilityManager.GrantAbility(abilityDefinition);
            
            bool success = Source.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(success, "Client-only ability should activate locally on client");
        }
        
        /// <summary>
        /// Verifies that calling EndAbility correctly terminates the ability and updates its active state.
        /// </summary>
        [Test]
        public void AbilityTests_EndAbility_SuccessfullyTerminatesAbility()
        {
            var abilityDefinition = AbilityUtilities.CreateInstantAbilityDefinition();
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            Source.AbilityManager.GrantAbility(abilityDefinition);
            Source.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(Source.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive);
            
            Source.AbilityManager.EndAbility(abilityDefinition.UniqueName);
            
            Assert.IsFalse(Source.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive, "Ability should no longer be active after calling EndAbility");
        }
        
        /// <summary>
        /// Verifies that predicted abilities play their associated cues with the correct prediction key.
        /// </summary>
        [Test]
        public void AbilityTests_PredictedActivation_PlaysCuesWithPredictionKey()
        {
            SourceMock.Setup(m => m.IsServer()).Returns(false);
            SourceMock.Setup(m => m.IsLocalClient()).Returns(true);
            
            var cueTag = new Tag("Cue.VFX.Explosion");
            var cueAsset = ScriptableObject.CreateInstance<CueDefinition>();
            cueAsset.CueTag = cueTag;
            
            var abilityDefinition = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDefinition.ActivationCues = new[] { cueAsset };
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientPredicted;
            Source.AbilityManager.GrantAbility(abilityDefinition);
            
            var predictionKey = new PredictionKey { currentKey = 777 };
            var ability = Source.AbilityManager.Abilities[abilityDefinition.UniqueName];
            
            ability.TryActivateAbility(predictionKey, new AbilityData());
            ability.PlayActivationCues();
            
            SourceMock.Verify(m => m.PlayCue(cueTag, It.Is<CueData>(d => d.PredictionKey.currentKey == 777), true), Times.Once);
        }
    }
}