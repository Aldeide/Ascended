using System.Reflection;
using GameplayTags.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace Plugins.GameplayTags.Tests.Runtime
{
    public class TagTest
    {
        [Test]
        public void TagTest_CreateGameplayTag_HasCorrectValues()
        {
            var tag = new Tag("Testing.Tag.TagA");

            var expectedAncestorNames = new[] { "Testing", "Testing.Tag" };
            var expectedAncestorHashCodes = new[] { -1702473778, 948346248 };
            try
            {
                Assert.AreEqual("Testing.Tag.TagA", tag.Name);
                Assert.AreEqual(85776883, tag.HashCode);
                Assert.AreEqual(expectedAncestorNames, tag.AncestorsNames);
                Assert.AreEqual(expectedAncestorHashCodes, tag.AncestorsHashCodes);
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Debug.Log(loaderException.Message);
                }
                throw;
            }
        }
        
        [Test]
        public void TagTest_GetName_ReturnsTagName()
        {
            var tag = new Tag("Testing.Tag.TagA");
            
            Assert.AreEqual("Testing.Tag.TagA", tag.Name);
        }
        
        [Test]
        public void TagTest_GetHashCode_ReturnsTagHashCode()
        {
            var tag = new Tag("Testing.Tag.TagA");
            
            Assert.AreEqual(85776883, tag.GetHashCode());
        }
        
        [TestCase("Testing.Tag.TagA", "Testing", true)]
        [TestCase("Testing.Tag.TagA", "Testing.Tag", true)]
        [TestCase("Testing.Tag.TagA", "Testing.Tag.TagA", true)]
        [TestCase("Testing.Tag.TagA", "Tag.TagA", true)]
        [TestCase("Testing.Tag.TagA", "TagA", true)]
        [TestCase("Testing.Tag.TagA", "TestingButDifferent", false)]
        [TestCase("Testing.Tag.TagA", "Testing.Tag.TagB", false)]
        [TestCase("Testing.Tag.TagA", "Testing.Tag.TagA.Extra", false)]
        [TestCase("Testing.Tag.TagA", "", true)]
        public void TagTest_HasTag_ReturnsExpectedResult(string mainTagName, string otherTagName, bool expectedResult)
        {
            var tag = new Tag(mainTagName);
            var otherTag = new Tag(otherTagName);
            
            Assert.That(tag.HasTag(otherTag), Is.EqualTo(expectedResult));
        }
        
        [Test]
        public void TagTest_IsAncestorOf_ReturnsTrueWhenIsAncestor()
        {
            var tag = new Tag("Testing");
            var otherTag = new Tag("Testing.Tag.TagA");
            
            Assert.IsTrue(tag.IsAncestorOf(otherTag));
        }
        
        [Test]
        public void TagTest_IsAncestorOf_ReturnsFalseWhenIsNotAncestor()
        {
            var tag = new Tag("Testing");
            var otherTag = new Tag("TestingButDifferent.Tag");
            
            Assert.IsFalse(tag.IsAncestorOf(otherTag));
        }
        
        [Test]
        public void TagTest_Operators_AreFunctional()
        {
            var tag = new Tag("Testing.Tag.TagA");
            var sameTag = new Tag("Testing.Tag.TagA");
            var differentTag = new Tag("Testing.Tag.TagB");
            
            Assert.IsTrue(tag == sameTag);
            Assert.IsFalse(tag == differentTag);
            Assert.IsTrue(tag != differentTag);
            Assert.IsFalse(tag != sameTag);
            Assert.IsTrue(tag.Equals(sameTag));
        }
    }
}