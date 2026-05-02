using GameplayTags.Runtime;
using NUnit.Framework;

namespace Plugins.Tags.Tests.Runtime
{
    public class TagQueryTest
    {
          [Test]
        public void TagQueryTest_AnyOfExactNonMatching_ReturnsFalse()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var testTagC = new Tag("Test.Tag.C");
            var conditionOne = new TagCondition(TagMatchType.AnyOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(conditionOne);
            
            Assert.IsFalse(tagQuery.MatchesTag(testTagC));
        }
        
        [Test]
        public void TagQueryTest_AnyOfExactMatching_ReturnsTrue()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var conditionOne = new TagCondition(TagMatchType.AnyOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(conditionOne);
            
            Assert.IsTrue(tagQuery.MatchesTag(testTagB));
        }
        
        [Test]
        public void TagQueryTest_AnyOfPartialNonMatching_ReturnsFalse()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var testTagC = new Tag("Test.Tag.C");
            var conditionOne = new TagCondition(TagMatchType.AnyOfPartial, testTagA, testTagB);
            var tagQuery = new TagQuery(conditionOne);
            
            Assert.IsFalse(tagQuery.MatchesTag(testTagC));
        }
        
        [Test]
        public void TagQueryTest_AnyOfPartialMatching_ReturnsTrue()
        {
            var testTagA = new Tag("Test.Tag");
            var testTagB = new Tag("Test.Tag.B");
            var testTagC = new Tag("Test.Tag.C");
            var conditionOne = new TagCondition(TagMatchType.AnyOfPartial, testTagA, testTagB);
            var tagQuery = new TagQuery(conditionOne);
            
            Assert.IsTrue(tagQuery.MatchesTag(testTagC));
        }
        
        [Test]
        public void TagQueryTest_AllOfExactNonMatching_ReturnsFalse()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var testTagC = new Tag("Test.Tag.C");
            var conditionOne = new TagCondition(TagMatchType.AllOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(conditionOne);
            
            Assert.IsFalse(tagQuery.MatchesTags(new[] { testTagA, testTagC }));
        }
        
        [Test]
        public void TagQueryTest_AllOfExactMatching_ReturnsTrue()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var conditionOne = new TagCondition(TagMatchType.AllOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(conditionOne);
            
            Assert.IsTrue(tagQuery.MatchesTags(new[] { testTagA, testTagB }));
        }
        
        [Test]
        public void TagQueryTest_AllOfPartialNonMatching_ReturnsFalse()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var testTagC = new Tag("Test.Tag.C");
            var conditionOne = new TagCondition(TagMatchType.AllOfPartial, testTagA, testTagB);
            var tagQuery = new TagQuery(conditionOne);
            
            Assert.IsFalse(tagQuery.MatchesTag(testTagC));
        }

        [Test]
        public void TagQueryTest_AllOfPartialMatching_ReturnsTrue()
        {
            var testTagA = new Tag("Test.Tag");
            var testTagB = new Tag("Test.Tag.B");
            var conditionOne = new TagCondition(TagMatchType.AllOfPartial, testTagA);
            var tagQuery = new TagQuery(conditionOne);

            Assert.IsTrue(tagQuery.MatchesTags(new[] { testTagB }));
        }

        [Test]
        public void TagQueryTest_NoneOfExactNonMatching_ReturnsTrue()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var testTagC = new Tag("Test.Tag.C");
            var conditionOne = new TagCondition(TagMatchType.NoneOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(conditionOne);

            Assert.IsTrue(tagQuery.MatchesTag(testTagC));
        }

        [Test]
        public void TagQueryTest_NoneOfExactMatching_ReturnsFalse()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var conditionOne = new TagCondition(TagMatchType.NoneOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(conditionOne);

            Assert.IsFalse(tagQuery.MatchesTag(testTagB));
        }

        [Test]
        public void TagQueryTest_NoneOfPartialNonMatching_ReturnsTrue()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var testTagC = new Tag("Test.Tag.C");
            var conditionOne = new TagCondition(TagMatchType.NoneOfPartial, testTagA, testTagB);
            var tagQuery = new TagQuery(conditionOne);

            Assert.IsTrue(tagQuery.MatchesTag(testTagC));
        }

        [Test]
        public void TagQueryTest_NoneOfPartialMatching_ReturnsFalse()
        {
            var testTagA = new Tag("Test.Tag");
            var testTagB = new Tag("Test.Tag.B");
            var conditionOne = new TagCondition(TagMatchType.NoneOfPartial, testTagA);
            var tagQuery = new TagQuery(conditionOne);

            Assert.IsFalse(tagQuery.MatchesTag(testTagB));
        }
    }
}
