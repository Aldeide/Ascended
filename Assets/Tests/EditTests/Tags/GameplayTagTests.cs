using NUnit.Framework;
using Systems.AbilitySystem.Attributes;
using Systems.AbilitySystem.Tags;

namespace Tests.EditTests.Tags
{
    public class GameplayTagTests
    {
        [Test]
        public void GameplayTagTests_CreateGameplayTag_HasCorrectValues()
        {
            GameplayTag tag = new GameplayTag("Testing.Tag.TagA");

            string[] expectedAncestorNames = new[] { "Testing", "Testing.Tag" };
            int[] expectedAncestorHashCodes = new[] { -1702473778, 948346248 };
            
            Assert.AreEqual("Testing.Tag.TagA", tag.Name);
            Assert.AreEqual(85776883, tag.HashCode);
            Assert.AreEqual(expectedAncestorNames, tag.AncestorNames);
            Assert.AreEqual(expectedAncestorHashCodes, tag.AncestorsHashCodes);
        }
        
        [Test]
        public void GameplayTagTests_GetName_ReturnsTagName()
        {
            GameplayTag tag = new GameplayTag("Testing.Tag.TagA");
            
            Assert.AreEqual("Testing.Tag.TagA", tag.GetName());
        }
        
        [Test]
        public void GameplayTagTests_GetHasCode_ReturnsTagHashCode()
        {
            GameplayTag tag = new GameplayTag("Testing.Tag.TagA");
            
            Assert.AreEqual(85776883, tag.GetHashCode());
        }
        
        [Test]
        public void GameplayTagTests_HasTag_ReturnsTrueWhenContainsOtherTag()
        {
            GameplayTag tag = new GameplayTag("Testing.Tag.TagA");
            GameplayTag otherTag = new GameplayTag("Testing");
            
            Assert.IsTrue(tag.HasTag(otherTag));
        }
        
        [Test]
        public void GameplayTagTests_HasTag_ReturnsFalseWhenDoesNotContainOtherTag()
        {
            GameplayTag tag = new GameplayTag("Testing.Tag.TagA");
            GameplayTag otherTag = new GameplayTag("TestingButDifferent");
            
            Assert.IsFalse(tag.HasTag(otherTag));
        }
        
        [Test]
        public void GameplayTagTests_IsAncestorOf_ReturnsTrueWhenIsAncestor()
        {
            GameplayTag tag = new GameplayTag("Testing");
            GameplayTag otherTag = new GameplayTag("Testing.Tag.TagA");
            
            Assert.IsTrue(tag.IsAncestorOf(otherTag));
        }
        
        [Test]
        public void GameplayTagTests_IsAncestorOf_ReturnsFalseWhenIsNotAncestor()
        {
            GameplayTag tag = new GameplayTag("Testing");
            GameplayTag otherTag = new GameplayTag("TestingButDifferent.Tag");
            
            Assert.IsFalse(tag.IsAncestorOf(otherTag));
        }
        
        [Test]
        public void GameplayTagTests_Operators_AreFunctional()
        {
            GameplayTag tag = new GameplayTag("Testing.Tag.TagA");
            GameplayTag sameTag = new GameplayTag("Testing.Tag.TagA");
            GameplayTag differentTag = new GameplayTag("Testing.Tag.TagB");
            
            Assert.IsTrue(tag == sameTag);
            Assert.IsFalse(tag == differentTag);
            Assert.IsTrue(tag != differentTag);
            Assert.IsFalse(tag != sameTag);
            Assert.IsTrue(tag.Equals(sameTag));
        }
    }
}