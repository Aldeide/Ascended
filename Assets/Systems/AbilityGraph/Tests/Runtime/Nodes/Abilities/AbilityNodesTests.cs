using AbilityGraph.Runtime.Nodes.Abilities;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using GameplayTags.Runtime;
using Moq;
using NUnit.Framework;

namespace AbilityGraph.Tests.Runtime.Nodes.Abilities
{
    public class AbilityNodesTests
    {
        private Mock<IAbilitySystem> _ownerMock;
        private Mock<GameplayTagManager> _tagManagerMock;
        private Mock<EffectManager> _effectManagerMock;
        private Mock<AttributeSetManager> _attributeManagerMock;
        private Mock<Ability> _abilityMock;

        [SetUp]
        public void Setup()
        {
            _ownerMock = new Mock<IAbilitySystem>();
            
            // EffectManager must be setup first because GameplayTagManager depends on it in its constructor
            _effectManagerMock = new Mock<EffectManager>(_ownerMock.Object);
            _ownerMock.Setup(o => o.EffectManager).Returns(_effectManagerMock.Object);

            _tagManagerMock = new Mock<GameplayTagManager>(_ownerMock.Object);
            _ownerMock.Setup(o => o.TagManager).Returns(_tagManagerMock.Object);

            _attributeManagerMock = new Mock<AttributeSetManager>(_ownerMock.Object);
            _ownerMock.Setup(o => o.AttributeSetManager).Returns(_attributeManagerMock.Object);

            _abilityMock = new Mock<Ability>();
            _abilityMock.Setup(a => a.Owner).Returns(_ownerMock.Object);
            _abilityMock.Setup(a => a.Level).Returns(5);
        }

        [Test]
        public void HasTagNode_ReturnsCorrectValue()
        {
            var node = new HasTagNode();
            node.Initialise(_abilityMock.Object);
            var tag = new Tag("Test.Tag");
            
            node.Tag = tag;
            _tagManagerMock.Setup(m => m.HasTag(tag)).Returns(true);
            
            node.OnProcess();
            Assert.IsTrue(node.HasTag);
            
            _tagManagerMock.Setup(m => m.HasTag(tag)).Returns(false);
            node.OnProcess();
            Assert.IsFalse(node.HasTag);
        }

        [Test]
        public void GetAbilityLevelNode_ReturnsCorrectLevel()
        {
            var node = new GetAbilityLevelNode();
            node.Initialise(_abilityMock.Object);
            
            node.OnProcess();
            Assert.AreEqual(5, node.Level);
        }

        [Test]
        public void ModifyAttributeBaseNode_AppliesChange()
        {
            var node = new ModifyAttributeBaseNode();
            node.Initialise(_abilityMock.Object);
            node.AttributeName = "Stat.Health";
            node.Value = 10f;
            node.ModificationType = AttributeModificationType.Add;

            var attrMock = new Mock<AbilitySystem.Runtime.Attributes.Attribute>();
            attrMock.Setup(a => a.BaseValue).Returns(100f);
            
            _attributeManagerMock.Setup(m => m.GetAttribute("Stat.Health")).Returns(attrMock.Object);

            node.OnProcess();
            
            attrMock.Verify(a => a.SetBaseValue(110f), Times.Once);
        }
    }
}
