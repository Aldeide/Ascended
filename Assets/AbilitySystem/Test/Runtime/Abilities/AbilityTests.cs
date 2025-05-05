using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.PassiveAbility;
using AbilitySystem.Runtime.Tags;
using static AbilitySystem.Test.Utilities.AbilityUtilities;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilityTests
    {
        [Test]
        public void AbilityTests_ActivateAbility_AppliesModifiers()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            Assert.True(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.uniqueName));
            var healthAttribute = owner.Object.AttributeSetManager.GetAttribute("Health");
            
            Assert.AreEqual(1000, healthAttribute.CurrentValue);
            Assert.AreEqual(100, healthAttribute.BaseValue);
        }
        
        [Test]
        public void AbilityTests_ActivateAbilityMissingRequiredTags_ActivationFails()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.ActivationRequiredTags = new GameplayTag[] { new("Tag.That.Is.Missing") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            
            Assert.IsFalse(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.uniqueName));
        }
        
        [Test]
        public void AbilityTests_ActivateAbilityWithRequiredTags_ActivationSucceeds()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.ActivationRequiredTags = new GameplayTag[] { new("Tag.That.Is.Required") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.TagManager.AddTag(new GameplayTag("Tag.That.Is.Required"));
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            
            Assert.IsTrue(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.uniqueName));
        }
        
        [Test]
        public void AbilityTests_ActivateAbilityWithBlockedTag_ActivationFails()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.ActivationBlockedTags = new[] { new GameplayTag("Blocking.Tag") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            owner.Object.TagManager.AddTag(new GameplayTag("Blocking.Tag"));
            
            Assert.IsFalse(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.uniqueName));
        }
        
        [Test]
        public void AbilityTests_ActivateAbilityWithoutBlockedTag_ActivationSucceeds()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.ActivationBlockedTags = new[] { new GameplayTag("Blocking.Tag") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            
            Assert.IsTrue(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.uniqueName));
        }
        
        [Test]
        public void AbilityTests_ActivateAbilityWithCancelTags_CancelsAbilityWithTag()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.CancelAbilityTags = new[] { new GameplayTag("Ability.Test") };
            var abilityDefinitionToCancel = CreateInstantAbilityDefinition();
            abilityDefinition.uniqueName = "AbilityToCancel";
            abilityDefinition.AssetTags = new[] { new GameplayTag("Ability.Test") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinitionToCancel);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            owner.Object.AbilityManager.TryActivateAbility(abilityDefinitionToCancel.uniqueName);
            owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.uniqueName);
            
            Assert.IsFalse(owner.Object.AbilityManager.Abilities["AbilityToCancel"].IsActive);
        }
        
        [Test]
        public void AbilityTests_ActivateWithOwnedTags_GrantsTags()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.ActivationOwnedTags = new[] { new GameplayTag("Ability.Grants.Tag") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.uniqueName);
            
            Assert.IsTrue(owner.Object.TagManager.HasTag(new GameplayTag("Ability.Grants.Tag")));
        }
        
        [Test]
        public void AbilityTests_ActivateLocalOnServer_ActivationFails()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.networkPolicy = AbilityNetworkPolicy.ClientOnly;
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            
            Assert.IsFalse(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.uniqueName));
        }
        
        [Test]
        public void AbilityTests_ActivateLocalOnClient_ActivationSuccess()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.networkPolicy = AbilityNetworkPolicy.ClientOnly;
            owner.Setup(m => m.IsServer()).Returns(false);
            owner.Setup(m => m.IsHost()).Returns(false);
            owner.Setup(m => m.IsLocalClient()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            
            Assert.IsTrue(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.uniqueName));
        }
        
        [Test]
        public void AbilityTests_EndAbility_EndsAbility()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            Assert.IsTrue(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.uniqueName));
            Assert.IsTrue(owner.Object.AbilityManager.Abilities[abilityDefinition.uniqueName].IsActive);
            
            owner.Object.AbilityManager.EndAbility(abilityDefinition.uniqueName);
            
            Assert.IsFalse(owner.Object.AbilityManager.Abilities[abilityDefinition.uniqueName].IsActive);
        }
    }
}