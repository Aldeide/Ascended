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
        
        [Test]
        public void TagTest_HasTag_ReturnsTrueWhenContainsOtherTag()
        {
            var tag = new Tag("Testing.Tag.TagA");
            var otherTag = new Tag("Testing");
            
            Assert.IsTrue(tag.HasTag(otherTag));
        }
        
        [Test]
        public void TagTest_HasTag_ReturnsFalseWhenDoesNotContainOtherTag()
        {
            var tag = new Tag("Testing.Tag.TagA");
            var otherTag = new Tag("TestingButDifferent");
            
            Assert.IsFalse(tag.HasTag(otherTag));
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