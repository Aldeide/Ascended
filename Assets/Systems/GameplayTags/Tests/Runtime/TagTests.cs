using GameplayTags.Runtime;
using NUnit.Framework;

namespace Plugins.Tags.Tests.Runtime
{
    /// <summary>
    /// Unit tests for the Tag struct, verifying hierarchical name parsing, hash code stability, and matching logic.
    /// </summary>
    public class TagTests
    {
        /// <summary>
        /// Verifies that creating a tag correctly parses its name and generates the expected ancestor list and hash codes.
        /// </summary>
        [Test]
        public void TagTests_Creation_ParsesHierarchyAndGeneratesHashes()
        {
            var tag = new Tag("Testing.Tag.TagA");

            var expectedAncestorNames = new[] { "Testing", "Testing.Tag" };
            var expectedAncestorHashCodes = new[] { -1702473778, 948346248 };
            
            Assert.AreEqual("Testing.Tag.TagA", tag.Name);
            Assert.AreEqual(85776883, tag.HashCode);
            Assert.AreEqual(expectedAncestorNames, tag.AncestorsNames);
            Assert.AreEqual(expectedAncestorHashCodes, tag.AncestorsHashCodes);
        }
        
        /// <summary>
        /// Verifies that GetName returns the correct full tag string.
        /// </summary>
        [Test]
        public void TagTests_GetName_ReturnsFullTagName()
        {
            var tag = new Tag("Testing.Tag.TagA");
            Assert.AreEqual("Testing.Tag.TagA", tag.Name);
        }
        
        /// <summary>
        /// Verifies that GetHashCode returns the stable precomputed hash code for the tag.
        /// </summary>
        [Test]
        public void TagTests_GetHashCode_ReturnsPrecomputedHash()
        {
            var tag = new Tag("Testing.Tag.TagA");
            Assert.AreEqual(85776883, tag.GetHashCode());
        }
        
        /// <summary>
        /// Exhaustive verification of the HasTag method, ensuring correct hierarchical matching.
        /// </summary>
        [TestCase("Testing.Tag.TagA", "Testing", true)]
        [TestCase("Testing.Tag.TagA", "Testing.Tag", true)]
        [TestCase("Testing.Tag.TagA", "Testing.Tag.TagA", true)]
        [TestCase("Testing.Tag.TagA", "Tag.TagA", true)]
        [TestCase("Testing.Tag.TagA", "TagA", true)]
        [TestCase("Testing.Tag.TagA", "TestingButDifferent", false)]
        [TestCase("Testing.Tag.TagA", "Testing.Tag.TagB", false)]
        [TestCase("Testing.Tag.TagA", "Testing.Tag.TagA.Extra", false)]
        [TestCase("Testing.Tag.TagA", "", true)]
        public void TagTests_HasTag_MatchesExpectedResults(string mainTagName, string otherTagName, bool expectedResult)
        {
            var tag = new Tag(mainTagName);
            var otherTag = new Tag(otherTagName);
            
            Assert.That(tag.HasTag(otherTag), Is.EqualTo(expectedResult));
        }
        
        /// <summary>
        /// Verifies that IsAncestorOf correctly identifies parent tags.
        /// </summary>
        [Test]
        public void TagTests_IsAncestorOf_ReturnsTrueForDirectAndDistantAncestors()
        {
            var ancestor = new Tag("Testing");
            var descendant = new Tag("Testing.Tag.TagA");
            
            Assert.IsTrue(ancestor.IsAncestorOf(descendant), "Root should be ancestor of leaf");
            
            var directParent = new Tag("Testing.Tag");
            Assert.IsTrue(directParent.IsAncestorOf(descendant), "Direct parent should be ancestor of child");
        }
        
        /// <summary>
        /// Verifies that IsAncestorOf correctly returns false for unrelated, sibling, or identical tags.
        /// </summary>
        [Test]
        public void TagTests_IsAncestorOf_ReturnsFalseForNonAncestors()
        {
            var tag = new Tag("Testing");
            var unrelated = new Tag("Other.Tag");
            var identical = new Tag("Testing");
            var child = new Tag("Testing.Tag");
            
            Assert.IsFalse(tag.IsAncestorOf(unrelated), "Unrelated tags should not have ancestor relationship");
            Assert.IsFalse(tag.IsAncestorOf(identical), "Tag is not an ancestor of itself");
            Assert.IsFalse(child.IsAncestorOf(tag), "Child is not an ancestor of its parent");
        }

        /// <summary>
        /// Verifies that equality and inequality operators correctly compare tags based on their identity.
        /// </summary>
        [Test]
        public void TagTests_EqualityOperators_FunctionCorrectly()
        {
            var tagA1 = new Tag("Testing.Tag.TagA");
            var tagA2 = new Tag("Testing.Tag.TagA");
            var tagB = new Tag("Testing.Tag.TagB");
            
            Assert.IsTrue(tagA1 == tagA2, "Identical tags should be equal");
            Assert.IsFalse(tagA1 == tagB, "Different tags should not be equal");
            Assert.IsTrue(tagA1 != tagB, "Different tags should be unequal");
            Assert.IsTrue(tagA1.Equals(tagA2), "Equals method should return true for identical tags");
        }

        /// <summary>
        /// Verifies that ToString returns the tag Name property.
        /// </summary>
        [Test]
        public void TagTests_ToString_ReturnsTagName()
        {
            var tag = new Tag("Testing.Tag.TagA");
            Assert.AreEqual("Testing.Tag.TagA", tag.ToString());
        }
    }
}
