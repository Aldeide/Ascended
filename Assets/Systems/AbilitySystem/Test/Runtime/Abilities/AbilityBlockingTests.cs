using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilityBlockingTests
    {
        [Test]
        public void AbilityBlocking_ActiveAbilityBlocksTaggedAbility_CannotActivate()
        {
            // Arrange
            var owner = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            var tagManager = owner.Object.TagManager;
            
            var targetTag = new Tag("Ability.Test.Blocked");
            
            // Ability that blocks "Ability.Test.Blocked"
            var blockerDef = AbilityUtilities.CreateInstantAbilityDefinition();
            blockerDef.UniqueName = "Blocker";
            blockerDef.BlockAbilityTags = new[] { targetTag };
            
            // Ability that IS "Ability.Test.Blocked"
            var blockedDef = AbilityUtilities.CreateInstantAbilityDefinition();
            blockedDef.UniqueName = "Blocked";
            blockedDef.AssetTags = new[] { targetTag };
            
            owner.Object.AbilityManager.GrantAbility(blockerDef);
            owner.Object.AbilityManager.GrantAbility(blockedDef);
            
            // Act
            var activated = owner.Object.AbilityManager.TryActivateAbility("Blocker");
            
            // Assert: Blocker is active
            Assert.IsTrue(activated, "Blocker ability should have activated on the server.");
            Assert.IsTrue(owner.Object.AbilityManager.Abilities["Blocker"].IsActive);
            
            // Act: Try activate blocked ability
            var success = owner.Object.AbilityManager.TryActivateAbility("Blocked");
            
            // Assert: Activation failed
            Assert.IsFalse(success, "Ability should have been blocked by the active 'Blocker' ability.");
        }

        [Test]
        public void AbilityBlocking_BlockingAbilityEnds_AllowsActivation()
        {
            // Arrange
            var owner = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            var targetTag = new Tag("Ability.Test.Blocked");
            
            var blockerDef = AbilityUtilities.CreateInstantAbilityDefinition();
            blockerDef.UniqueName = "Blocker";
            blockerDef.BlockAbilityTags = new[] { targetTag };
            
            var blockedDef = AbilityUtilities.CreateInstantAbilityDefinition();
            blockedDef.UniqueName = "Blocked";
            blockedDef.AssetTags = new[] { targetTag };
            
            owner.Object.AbilityManager.GrantAbility(blockerDef);
            owner.Object.AbilityManager.GrantAbility(blockedDef);
            
            // Act: Fire blocker then end it
            var activated = owner.Object.AbilityManager.TryActivateAbility("Blocker");
            Assert.IsTrue(activated, "Blocker should activate.");
            owner.Object.AbilityManager.EndAbility("Blocker");
            
            // Act: Try activate previously blocked ability
            var success = owner.Object.AbilityManager.TryActivateAbility("Blocked");
            
            // Assert: Activation succeeded
            Assert.IsTrue(success, "Ability should be permitted to activate after the blocker has ended.");
        }
    }
}
