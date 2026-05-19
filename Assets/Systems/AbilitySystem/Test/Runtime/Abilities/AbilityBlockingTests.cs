using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Abilities
{
    /// <summary>
    /// Unit tests for ability blocking logic, ensuring that active abilities can prevent others from starting based on tag-based rules.
    /// </summary>
    public class AbilityBlockingTests : AbilitySystemTestBase
    {
        private static readonly Tag TargetBlockedTag = new Tag("Ability.Test.Blocked");

        /// <summary>
        /// Verifies that an active ability with 'BlockAbilityTags' correctly prevents the activation of any other ability that possesses one of those tags.
        /// </summary>
        [Test]
        public void AbilityBlockingTests_ActiveBlocker_BlocksMatchingTaggedAbility()
        {
            // Ability that blocks "Ability.Test.Blocked"
            var blockerDef = AbilityUtilities.CreateInstantAbilityDefinition();
            blockerDef.UniqueName = "Blocker";
            blockerDef.BlockAbilityTags = new[] { TargetBlockedTag };
            
            // Ability that IS "Ability.Test.Blocked"
            var blockedDef = AbilityUtilities.CreateInstantAbilityDefinition();
            blockedDef.UniqueName = "Blocked";
            blockedDef.AssetTags = new[] { TargetBlockedTag };
            
            Source.AbilityManager.GrantAbility(blockerDef);
            Source.AbilityManager.GrantAbility(blockedDef);
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            
            // Activate blocker
            bool blockerSuccess = Source.AbilityManager.TryActivateAbility("Blocker");
            Assert.IsTrue(blockerSuccess, "Blocker ability should have activated");
            Assert.IsTrue(Source.AbilityManager.Abilities["Blocker"].IsActive);
            
            // Try activate blocked ability
            bool blockedSuccess = Source.AbilityManager.TryActivateAbility("Blocked");
            Assert.IsFalse(blockedSuccess, "Ability should have been blocked by the active 'Blocker' ability");
        }

        /// <summary>
        /// Verifies that an ability that was previously blocked becomes available for activation once the blocking ability has ended.
        /// </summary>
        [Test]
        public void AbilityBlockingTests_BlockerEnded_AllowsPreviouslyBlockedAbilityToActivate()
        {
            var blockerDef = AbilityUtilities.CreateInstantAbilityDefinition();
            blockerDef.UniqueName = "Blocker";
            blockerDef.BlockAbilityTags = new[] { TargetBlockedTag };
            
            var blockedDef = AbilityUtilities.CreateInstantAbilityDefinition();
            blockedDef.UniqueName = "Blocked";
            blockedDef.AssetTags = new[] { TargetBlockedTag };
            
            Source.AbilityManager.GrantAbility(blockerDef);
            Source.AbilityManager.GrantAbility(blockedDef);
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            
            // Activate and then immediately end the blocker
            Source.AbilityManager.TryActivateAbility("Blocker");
            Source.AbilityManager.EndAbility("Blocker");
            
            // Try activate previously blocked ability
            bool success = Source.AbilityManager.TryActivateAbility("Blocked");
            
            Assert.IsTrue(success, "Ability should be permitted to activate after the blocker has ended");
        }

        /// <summary>
        /// Verifies that blocking a parent tag correctly prevents the activation of an ability that possesses a descendant/sub-tag of that parent.
        /// </summary>
        [Test]
        public void AbilityBlockingTests_ActiveBlocker_BlocksDescendantTaggedAbility()
        {
            var parentTag = new Tag("Ability.Test");
            var childTag = new Tag("Ability.Test.SubAction");

            var blockerDef = AbilityUtilities.CreateInstantAbilityDefinition();
            blockerDef.UniqueName = "Blocker";
            blockerDef.BlockAbilityTags = new[] { parentTag };
            
            var blockedDef = AbilityUtilities.CreateInstantAbilityDefinition();
            blockedDef.UniqueName = "Blocked";
            blockedDef.AssetTags = new[] { childTag };
            
            Source.AbilityManager.GrantAbility(blockerDef);
            Source.AbilityManager.GrantAbility(blockedDef);
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            
            // Activate blocker
            bool blockerSuccess = Source.AbilityManager.TryActivateAbility("Blocker");
            Assert.IsTrue(blockerSuccess, "Blocker ability should have activated");
            Assert.IsTrue(Source.AbilityManager.Abilities["Blocker"].IsActive);
            
            // Try activate blocked ability with sub-tag
            bool blockedSuccess = Source.AbilityManager.TryActivateAbility("Blocked");
            Assert.IsFalse(blockedSuccess, "Ability with descendant tag should have been blocked by parent tag blocker");
        }
    }
}
