using AbilitySystem.Runtime.Tags;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Tags
{
    public class GameplayTagQueryTests
    {
        [Test]
        public void GameplayTagQueryTests_AnyOfExactNonMatching_ReturnsFalse()
        {
            var testTagA = new GameplayTag("Test.Tag.A");
            var testTagB = new GameplayTag("Test.Tag.B");
            var testTagC = new GameplayTag("Test.Tag.C");
            var conditionOne = new GameplayTagCondition(GameplayTagMatchType.AnyOfExact, testTagA, testTagB);
            var tagQuery = new GameplayTagQuery(conditionOne);
            
            Assert.IsFalse(tagQuery.MatchesTag(testTagC));
        }
        
        [Test]
        public void GameplayTagQueryTests_AnyOfExactMatching_ReturnsTrue()
        {
            var testTagA = new GameplayTag("Test.Tag.A");
            var testTagB = new GameplayTag("Test.Tag.B");
            var conditionOne = new GameplayTagCondition(GameplayTagMatchType.AnyOfExact, testTagA, testTagB);
            var tagQuery = new GameplayTagQuery(conditionOne);
            
            Assert.IsTrue(tagQuery.MatchesTag(testTagB));
        }
        
        [Test]
        public void GameplayTagQueryTests_AnyOfPartialNonMatching_ReturnsFalse()
        {
            var testTagA = new GameplayTag("Test.Tag.A");
            var testTagB = new GameplayTag("Test.Tag.B");
            var testTagC = new GameplayTag("Test.Tag.C");
            var conditionOne = new GameplayTagCondition(GameplayTagMatchType.AnyOfPartial, testTagA, testTagB);
            var tagQuery = new GameplayTagQuery(conditionOne);
            
            Assert.IsFalse(tagQuery.MatchesTag(testTagC));
        }
        
        [Test]
        public void GameplayTagQueryTests_AnyOfPartialMatching_ReturnsTrue()
        {
            var testTagA = new GameplayTag("Test.Tag");
            var testTagB = new GameplayTag("Test.Tag.B");
            var testTagC = new GameplayTag("Test.Tag.C");
            var conditionOne = new GameplayTagCondition(GameplayTagMatchType.AnyOfPartial, testTagA, testTagB);
            var tagQuery = new GameplayTagQuery(conditionOne);
            
            Assert.IsTrue(tagQuery.MatchesTag(testTagC));
        }
        
        [Test]
        public void GameplayTagQueryTests_AllOfExactNonMatching_ReturnsFalse()
        {
            var testTagA = new GameplayTag("Test.Tag.A");
            var testTagB = new GameplayTag("Test.Tag.B");
            var testTagC = new GameplayTag("Test.Tag.C");
            var conditionOne = new GameplayTagCondition(GameplayTagMatchType.AllOfExact, testTagA, testTagB);
            var tagQuery = new GameplayTagQuery(conditionOne);
            
            Assert.IsFalse(tagQuery.MatchesTags(new[] { testTagA, testTagC }));
        }
        
        [Test]
        public void GameplayTagQueryTests_AllOfExactMatching_ReturnsTrue()
        {
            var testTagA = new GameplayTag("Test.Tag.A");
            var testTagB = new GameplayTag("Test.Tag.B");
            var conditionOne = new GameplayTagCondition(GameplayTagMatchType.AllOfExact, testTagA, testTagB);
            var tagQuery = new GameplayTagQuery(conditionOne);
            
            Assert.IsTrue(tagQuery.MatchesTags(new[] { testTagA, testTagB }));
        }
        
        [Test]
        public void GameplayTagQueryTests_AllOfPartialNonMatching_ReturnsFalse()
        {
            var testTagA = new GameplayTag("Test.Tag.A");
            var testTagB = new GameplayTag("Test.Tag.B");
            var testTagC = new GameplayTag("Test.Tag.C");
            var conditionOne = new GameplayTagCondition(GameplayTagMatchType.AllOfPartial, testTagA, testTagB);
            var tagQuery = new GameplayTagQuery(conditionOne);
            
            Assert.IsFalse(tagQuery.MatchesTag(testTagC));
        }
        
        /*
         AllOfPartial makes little sense, a tag can't partially match multiple different tags.
        [Test]
        public void GameplayTagQueryTests_AllOfPartialMatching_ReturnsTrue()
        {
            var testTagA = new GameplayTag("Test.Tag.A");
            var testTagB = new GameplayTag("Test.Tag.B");
            var testTagC = new GameplayTag("Test.Tag");
            var conditionOne = new GameplayTagCondition(GameplayTagMatchType.AllOfPartial, testTagA, testTagB);
            var tagQuery = new GameplayTagQuery(conditionOne);
            
            Assert.IsTrue(tagQuery.MatchesTag(testTagC));
        }
        */
    }
}