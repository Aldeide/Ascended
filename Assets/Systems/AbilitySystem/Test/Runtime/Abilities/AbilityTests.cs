using AbilitySystem.Runtime.Abilities;
using GameplayTags.Runtime;
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
            Assert.True(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
            var healthAttribute = owner.Object.AttributeSetManager.GetAttribute("Health");
            
            Assert.AreEqual(1000, healthAttribute.CurrentValue);
            Assert.AreEqual(100, healthAttribute.BaseValue);
        }
        
        [Test]
        public void AbilityTests_ActivateAbilityMissingRequiredTags_ActivationFails()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.ActivationRequiredTags = new Tag[] { new("Tag.That.Is.Missing") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            
            Assert.IsFalse(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }
        
        [Test]
        public void AbilityTests_ActivateAbilityWithRequiredTags_ActivationSucceeds()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.ActivationRequiredTags = new Tag[] { new("Tag.That.Is.Required") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.TagManager.AddTag(new Tag("Tag.That.Is.Required"));
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            
            Assert.IsTrue(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }
        
        [Test]
        public void AbilityTests_ActivateAbilityWithBlockedTag_ActivationFails()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.ActivationBlockedTags = new[] { new Tag("Blocking.Tag") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            owner.Object.TagManager.AddTag(new Tag("Blocking.Tag"));
            
            Assert.IsFalse(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }
        
        [Test]
        public void AbilityTests_ActivateAbilityWithoutBlockedTag_ActivationSucceeds()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.ActivationBlockedTags = new[] { new Tag("Blocking.Tag") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            
            Assert.IsTrue(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }
        
        [Test]
        public void AbilityTests_ActivateAbilityWithCancelTags_CancelsAbilityWithTag()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.CancelAbilityTags = new[] { new Tag("Ability.Test") };
            var abilityDefinitionToCancel = CreateInstantAbilityDefinition();
            abilityDefinition.UniqueName = "AbilityToCancel";
            abilityDefinition.AssetTags = new[] { new Tag("Ability.Test") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinitionToCancel);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            owner.Object.AbilityManager.TryActivateAbility(abilityDefinitionToCancel.UniqueName);
            owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsFalse(owner.Object.AbilityManager.Abilities["AbilityToCancel"].IsActive);
        }
        
        [Test]
        public void AbilityTests_ActivateWithOwnedTags_GrantsTags()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.ActivationOwnedTags = new[] { new Tag("Ability.Grants.Tag") };
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName);
            
            Assert.IsTrue(owner.Object.TagManager.HasTag(new Tag("Ability.Grants.Tag")));
        }
        
        [Test]
        public void AbilityTests_ActivateLocalOnServer_ActivationFails()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            
            Assert.IsFalse(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }
        
        [Test]
        public void AbilityTests_ActivateLocalOnClient_ActivationSuccess()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.NetworkPolicy = AbilityNetworkPolicy.ClientOnly;
            owner.Setup(m => m.IsServer()).Returns(false);
            owner.Setup(m => m.IsHost()).Returns(false);
            owner.Setup(m => m.IsLocalClient()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            
            Assert.IsTrue(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
        }
        
        [Test]
        public void AbilityTests_EndAbility_EndsAbility()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            Assert.IsTrue(owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.UniqueName));
            Assert.IsTrue(owner.Object.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive);
            
            owner.Object.AbilityManager.EndAbility(abilityDefinition.UniqueName);
            
            Assert.IsFalse(owner.Object.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive);
        }
    }
}