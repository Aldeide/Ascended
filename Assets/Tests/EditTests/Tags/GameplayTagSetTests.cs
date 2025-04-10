using NUnit.Framework;
using Systems.AbilitySystem.Tags;

namespace Tests.EditTests.Tags
{
    public class GameplayTagSetTests
    {
        [Test]
        public void GameplayTagSetTests_CreateGameplayTagSetWithStringArray_HasCorrectValues()
        {
            string[] tags = new[] { "Unit.Player", "Ability.Passive" };
            GameplayTagSet set = new GameplayTagSet(tags);
            
            Assert.AreEqual(2, set.Tags.Length);
            Assert.AreEqual("Unit.Player", set.Tags[0].Name);
            Assert.AreEqual("Ability.Passive", set.Tags[1].Name);
        }
        
        [Test]
        public void GameplayTagSetTests_CreateGameplayTagSetWithTagArray_HasCorrectValues()
        {
            GameplayTag unitPlayerTag = new GameplayTag("Unit.Player");
            GameplayTag abilityPassiveTag = new GameplayTag("Ability.Passive");
            GameplayTag[] tags = new[] { unitPlayerTag, abilityPassiveTag };
            GameplayTagSet set = new GameplayTagSet(tags);
            
            Assert.AreEqual(2, set.Tags.Length);
            Assert.AreEqual("Unit.Player", set.Tags[0].Name);
            Assert.AreEqual("Ability.Passive", set.Tags[1].Name);
        }
        
        [Test]
        public void GameplayTagSetTests_HasTag_ReturnsTrueWhenContainsTag()
        {
            GameplayTag unitPlayerTag = new GameplayTag("Unit.Player");
            GameplayTag abilityPassiveTag = new GameplayTag("Ability.Passive");
            GameplayTag[] tags = new[] { unitPlayerTag, abilityPassiveTag };
            GameplayTagSet set = new GameplayTagSet(tags);
            
            Assert.IsTrue(set.HasTag(unitPlayerTag));
        }
        
        [Test]
        public void GameplayTagSetTests_HasTag_ReturnsFalseWhenDoesNotContainTag()
        {
            GameplayTag unitPlayerTag = new GameplayTag("Unit.Player");
            GameplayTag abilityPassiveTag = new GameplayTag("Ability.Passive");
            GameplayTag abilityOnHitTag = new GameplayTag("Ability.OnHit");
            GameplayTag[] tags = new[] { unitPlayerTag, abilityPassiveTag };
            GameplayTagSet set = new GameplayTagSet(tags);
            
            Assert.IsFalse(set.HasTag(abilityOnHitTag));
        }
        
        [Test]
        public void GameplayTagSetTests_HasAllTags_ReturnsTrueWhenContainsAllTags()
        {
            GameplayTag unitPlayerTag = new GameplayTag("Unit.Player");
            GameplayTag abilityPassiveTag = new GameplayTag("Ability.Passive");
            GameplayTag[] tags = new[] { unitPlayerTag, abilityPassiveTag };
            GameplayTagSet set = new GameplayTagSet(tags);
            GameplayTagSet otherSet = new GameplayTagSet(tags);
            
            Assert.IsTrue(set.HasAllTags(otherSet));
        }
        
        [Test]
        public void GameplayTagSetTests_HasAllTags_ReturnsFalseWhenDoesNotContainAllTags()
        {
            GameplayTag unitPlayerTag = new GameplayTag("Unit.Player");
            GameplayTag abilityPassiveTag = new GameplayTag("Ability.Passive");
            GameplayTag abilityOnHitTag = new GameplayTag("Ability.OnHit");
            GameplayTag[] tags = new[] { unitPlayerTag, abilityPassiveTag };
            GameplayTagSet set = new GameplayTagSet(tags);
            GameplayTagSet otherSet = new GameplayTagSet(unitPlayerTag, abilityOnHitTag);
            
            Assert.IsFalse(set.HasAllTags(otherSet));
        }
        
        [Test]
        public void GameplayTagSetTests_HasAnyTag_ReturnsTrueWhenContainsAnyTag()
        {
            GameplayTag unitPlayerTag = new GameplayTag("Unit.Player");
            GameplayTag abilityPassiveTag = new GameplayTag("Ability.Passive");
            GameplayTag abilityOnHitTag = new GameplayTag("Ability.OnHit");
            GameplayTag[] tags = new[] { unitPlayerTag, abilityPassiveTag };
            GameplayTagSet set = new GameplayTagSet(tags);
            GameplayTagSet otherSet = new GameplayTagSet(unitPlayerTag, abilityOnHitTag);
            
            Assert.IsTrue(set.HasAnyTags(otherSet));
        }
        
        [Test]
        public void GameplayTagSetTests_HasAnyTag_ReturnsFalseWhenDoesNotContainAnyTag()
        {
            GameplayTag unitPlayerTag = new GameplayTag("Unit.Player");
            GameplayTag abilityPassiveTag = new GameplayTag("Ability.Passive");
            GameplayTag abilityOnHitTag = new GameplayTag("Ability.OnHit");
            GameplayTag[] tags = new[] { unitPlayerTag, abilityPassiveTag };
            GameplayTagSet set = new GameplayTagSet(tags);
            GameplayTagSet otherSet = new GameplayTagSet(abilityOnHitTag);
            
            Assert.IsFalse(set.HasAnyTags(otherSet));
        }
        
        [Test]
        public void GameplayTagSetTests_HasNoneTags_ReturnsTrueWhenHasNoneTags()
        {
            GameplayTag unitPlayerTag = new GameplayTag("Unit.Player");
            GameplayTag abilityPassiveTag = new GameplayTag("Ability.Passive");
            GameplayTag abilityOnHitTag = new GameplayTag("Ability.OnHit");
            GameplayTag[] tags = new[] { unitPlayerTag, abilityPassiveTag };
            GameplayTagSet set = new GameplayTagSet(tags);
            GameplayTagSet otherSet = new GameplayTagSet(abilityOnHitTag);
            
            Assert.IsTrue(set.HasNoneTags(otherSet));
        }
        
        [Test]
        public void GameplayTagSetTests_HasNoneTags_ReturnsFalseWhenHasSomeTags()
        {
            GameplayTag unitPlayerTag = new GameplayTag("Unit.Player");
            GameplayTag abilityPassiveTag = new GameplayTag("Ability.Passive");
            GameplayTag abilityOnHitTag = new GameplayTag("Ability.OnHit");
            GameplayTag[] tags = new[] { unitPlayerTag, abilityPassiveTag };
            GameplayTagSet set = new GameplayTagSet(tags);
            GameplayTagSet otherSet = new GameplayTagSet(unitPlayerTag, abilityOnHitTag);
            
            Assert.IsFalse(set.HasNoneTags(otherSet));
        }
    }
}