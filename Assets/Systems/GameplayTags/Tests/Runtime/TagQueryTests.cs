using GameplayTags.Runtime;
using NUnit.Framework;

namespace Plugins.Tags.Tests.Runtime
{
    /// <summary>
    /// Unit tests for TagQuery, verifying complex tag matching logic across various match types and conditions.
    /// </summary>
    public class TagQueryTests
    {
        /// <summary>
        /// Verifies that 'AnyOfExact' correctly returns false when the provided tag does not exactly match any tags in the condition.
        /// </summary>
        [Test]
        public void TagQueryTests_AnyOfExactNonMatching_ReturnsFalse()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var testTagC = new Tag("Test.Tag.C");
            var condition = new TagCondition(TagMatchType.AnyOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(condition);
            
            Assert.IsFalse(tagQuery.MatchesTag(testTagC), "Query should not match an unrelated tag");
        }
        
        /// <summary>
        /// Verifies that 'AnyOfExact' correctly returns true when the provided tag exactly matches one of the tags in the condition.
        /// </summary>
        [Test]
        public void TagQueryTests_AnyOfExactMatching_ReturnsTrue()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var condition = new TagCondition(TagMatchType.AnyOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(condition);
            
            Assert.IsTrue(tagQuery.MatchesTag(testTagB), "Query should match a tag that is exactly in the list");
        }
        
        /// <summary>
        /// Verifies that 'AnyOfPartial' correctly returns true when the provided tag is a child of one of the tags in the condition.
        /// </summary>
        [Test]
        public void TagQueryTests_AnyOfPartialMatching_ReturnsTrue()
        {
            var testTagA = new Tag("Test.Tag");
            var testTagB = new Tag("Test.Tag.B");
            var testTagC = new Tag("Test.Tag.C");
            var condition = new TagCondition(TagMatchType.AnyOfPartial, testTagA, testTagB);
            var tagQuery = new TagQuery(condition);
            
            Assert.IsTrue(tagQuery.MatchesTag(testTagC), "Query should match a child tag during partial matching");
        }
        
        /// <summary>
        /// Verifies that 'AllOfExact' correctly returns true only when all tags in the condition are present in the provided list.
        /// </summary>
        [Test]
        public void TagQueryTests_AllOfExactMatching_ReturnsTrue()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var condition = new TagCondition(TagMatchType.AllOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(condition);
            
            Assert.IsTrue(tagQuery.MatchesTags(new[] { testTagA, testTagB }), "Query should match when all exact tags are present");
        }
        
        /// <summary>
        /// Verifies that 'AllOfExact' returns false if any required tag is missing from the provided list.
        /// </summary>
        [Test]
        public void TagQueryTests_AllOfExactMissingOne_ReturnsFalse()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var testTagC = new Tag("Test.Tag.C");
            var condition = new TagCondition(TagMatchType.AllOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(condition);
            
            Assert.IsFalse(tagQuery.MatchesTags(new[] { testTagA, testTagC }), "Query should not match if one of the exact tags is missing");
        }

        /// <summary>
        /// Verifies that 'NoneOfExact' correctly returns true when none of the tags in the provided list are present in the condition.
        /// </summary>
        [Test]
        public void TagQueryTests_NoneOfExactNonMatching_ReturnsTrue()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var testTagC = new Tag("Test.Tag.C");
            var condition = new TagCondition(TagMatchType.NoneOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(condition);

            Assert.IsTrue(tagQuery.MatchesTag(testTagC), "Query should return true if the tag is not in the forbidden list");
        }

        /// <summary>
        /// Verifies that 'NoneOfExact' returns false if a forbidden tag is exactly matched.
        /// </summary>
        [Test]
        public void TagQueryTests_NoneOfExactMatching_ReturnsFalse()
        {
            var testTagA = new Tag("Test.Tag.A");
            var testTagB = new Tag("Test.Tag.B");
            var condition = new TagCondition(TagMatchType.NoneOfExact, testTagA, testTagB);
            var tagQuery = new TagQuery(condition);

            Assert.IsFalse(tagQuery.MatchesTag(testTagB), "Query should return false if a forbidden exact tag is present");
        }

        /// <summary>
        /// Verifies that 'NoneOfPartial' returns false if the provided tag is a child of any forbidden tag.
        /// </summary>
        [Test]
        public void TagQueryTests_NoneOfPartialMatching_ReturnsFalse()
        {
            var testTagA = new Tag("Test.Tag");
            var testTagB = new Tag("Test.Tag.B");
            var condition = new TagCondition(TagMatchType.NoneOfPartial, testTagA);
            var tagQuery = new TagQuery(condition);

            Assert.IsFalse(tagQuery.MatchesTag(testTagB), "Query should return false if the tag is a child of a forbidden partial tag");
        }
    }
}
