using AbilityGraph.Runtime.Nodes.Abilities;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using Moq;
using NUnit.Framework;

namespace AbilityGraph.Tests.Runtime.Nodes.Abilities
{
    /// <summary>
    /// Tests for core Ability Graph nodes that interact with the Ability System (tags, levels, attributes).
    /// </summary>
    public class AbilityNodesTests : AbilitySystemTestBase
    {
        private Mock<Ability> _abilityMock;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _abilityMock = new Mock<Ability>();
            _abilityMock.Setup(a => a.Owner).Returns(Source);
            _abilityMock.Setup(a => a.Level).Returns(5);
        }

        /// <summary>
        /// Validates that HasTagNode correctly identifies the presence or absence of a tag on the owner.
        /// </summary>
        [Test]
        public void AbilityNodesTests_HasTagNode_ReturnsCorrectTagStatus()
        {
            var node = new HasTagNode();
            var context = new AbilityGraph.Runtime.GraphContext(_abilityMock.Object, Source);
            node.Initialise(context);
            var tag = new Tag("Test.Tag");
            
            var tagManagerMock = new Mock<GameplayTagManager>(Source);
            SourceMock.Setup(m => m.TagManager).Returns(tagManagerMock.Object);
            
            node.Tag = tag;
            tagManagerMock.Setup(m => m.HasTag(tag)).Returns(true);
            
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsTrue(node.HasTag);
            
            tagManagerMock.Setup(m => m.HasTag(tag)).Returns(false);
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsFalse(node.HasTag);
        }

        /// <summary>
        /// Validates that GetAbilityLevelNode correctly retrieves the level of the associated ability.
        /// </summary>
        [Test]
        public void AbilityNodesTests_GetAbilityLevelNode_ReturnsCorrectAbilityLevel()
        {
            var node = new GetAbilityLevelNode();
            var context = new AbilityGraph.Runtime.GraphContext(_abilityMock.Object, Source);
            node.Initialise(context);
            
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(5, node.Level);
        }

        /// <summary>
        /// Validates that ModifyAttributeBaseNode correctly applies modifications to an attribute's base value.
        /// </summary>
        [Test]
        public void AbilityNodesTests_ModifyAttributeBaseNode_AppliesValueModification()
        {
            var node = new ModifyAttributeBaseNode();
            var context = new AbilityGraph.Runtime.GraphContext(_abilityMock.Object, Source);
            node.Initialise(context);
            node.AttributeName = "Stat.Health";
            node.Value = 10f;
            node.ModificationType = AttributeModificationType.Add;

            var attrMock = new Mock<AbilitySystem.Runtime.Attributes.Attribute>();
            attrMock.Setup(a => a.BaseValue).Returns(100f);
            
            var attributeManagerMock = new Mock<AttributeSetManager>(Source);
            SourceMock.Setup(m => m.AttributeSetManager).Returns(attributeManagerMock.Object);
            attributeManagerMock.Setup(m => m.GetAttribute("Stat.Health")).Returns(attrMock.Object);

            AbilityGraphTestUtilities.InvokeProcess(node);
            
            attrMock.Verify(a => a.SetBaseValue(110f), Times.Once);
        }
    }
}
