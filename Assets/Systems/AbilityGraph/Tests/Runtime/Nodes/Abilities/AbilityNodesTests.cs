using AbilityGraph.Runtime;
using AbilityGraph.Runtime.Nodes.Abilities;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using Moq;
using NUnit.Framework;
using System.Reflection;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Abilities
{
    /// <summary>
    /// Tests for core Ability Graph nodes that interact with the Ability System (tags, levels, attributes).
    /// </summary>
    public class AbilityNodesTests : AbilitySystemTestBase
    {
        private Mock<Ability> _abilityMock;
        private GraphContext _context;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var def = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            def.UniqueName = "TestAbility";
            _abilityMock = new Mock<Ability>(def, Source, 5);
            _abilityMock.Setup(a => a.Owner).Returns(Source);
            _abilityMock.Setup(a => a.Level).Returns(5);
            _context = new GraphContext(_abilityMock.Object, Source);
        }

        /// <summary>
        /// Validates that HasTagNode correctly identifies the presence or absence of a tag on the owner.
        /// </summary>
        [Test]
        public void AbilityNodesTests_HasTagNode_ReturnsCorrectTagStatus()
        {
            var node = new HasTagNode();
            node.Initialise(_context);
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
            node.Initialise(_context);
            
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
            node.Initialise(_context);
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

        /// <summary>
        /// Validates that AddTagToOwnerNode correctly adds a gameplay tag to the owner.
        /// </summary>
        [Test]
        public void AbilityNodesTests_AddTagToOwnerNode_AddsTagToOwner()
        {
            var node = new AddTagToOwnerNode();
            node.Initialise(_context);

            var tag = new Tag("Test.TagToAdd");
            node.Tag = tag;

            Assert.IsFalse(Source.TagManager.HasTag(tag));

            AbilityGraphTestUtilities.InvokeProcess(node);

            Assert.IsTrue(Source.TagManager.HasTag(tag));
        }

        /// <summary>
        /// Validates that RemoveTagFromOwnerNode correctly removes a gameplay tag from the owner.
        /// </summary>
        [Test]
        public void AbilityNodesTests_RemoveTagFromOwnerNode_RemovesTagFromOwner()
        {
            var node = new RemoveTagFromOwnerNode();
            node.Initialise(_context);

            var tag = new Tag("Test.TagToRemove");
            node.Tag = tag;

            Source.TagManager.AddTag(tag);
            Assert.IsTrue(Source.TagManager.HasTag(tag));

            AbilityGraphTestUtilities.InvokeProcess(node);

            Assert.IsFalse(Source.TagManager.HasTag(tag));
        }

        /// <summary>
        /// Validates that GetAttributePercentNode correctly calculates the percentage ratio between two attributes.
        /// </summary>
        [Test]
        public void AbilityNodesTests_GetAttributePercentNode_ReturnsCorrectPercentage()
        {
            var node = new GetAttributePercentNode();
            node.Initialise(_context);

            node.CurrentAttributeFullName = "Stat.Health";
            node.MaxAttributeFullName = "Stat.MaxHealth";

            var currentAttrMock = new Mock<AbilitySystem.Runtime.Attributes.Attribute>();
            currentAttrMock.Setup(a => a.CurrentValue).Returns(50f);

            var maxAttrMock = new Mock<AbilitySystem.Runtime.Attributes.Attribute>();
            maxAttrMock.Setup(a => a.CurrentValue).Returns(100f);

            var attributeManagerMock = new Mock<AttributeSetManager>(Source);
            SourceMock.Setup(m => m.AttributeSetManager).Returns(attributeManagerMock.Object);
            attributeManagerMock.Setup(m => m.GetAttribute("Health")).Returns(currentAttrMock.Object);
            attributeManagerMock.Setup(m => m.GetAttribute("MaxHealth")).Returns(maxAttrMock.Object);

            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(0.5f, node.Percent);
        }

        /// <summary>
        /// Validates that WaitInputPressNode finishes execution when the ability receives an input pressed notification.
        /// </summary>
        [Test]
        public void AbilityNodesTests_WaitInputPressNode_FinishesOnInputPress()
        {
            var node = new WaitInputPressNode();
            node.Initialise(_context);

            var finished = false;
            node.onProcessFinished += (n) => finished = true;

            AbilityGraphTestUtilities.InvokeProcess(node);

            // Call input pressed on the ability
            _abilityMock.Object.NotifyInputPressed();

            Assert.IsTrue(finished);
        }

        /// <summary>
        /// Validates that WaitInputReleaseNode finishes execution when the ability receives an input released notification.
        /// </summary>
        [Test]
        public void AbilityNodesTests_WaitInputReleaseNode_FinishesOnInputRelease()
        {
            var node = new WaitInputReleaseNode();
            node.Initialise(_context);

            var finished = false;
            node.onProcessFinished += (n) => finished = true;

            AbilityGraphTestUtilities.InvokeProcess(node);

            // Call input released on the ability
            _abilityMock.Object.NotifyInputReleased();

            Assert.IsTrue(finished);
        }

        /// <summary>
        /// Validates that WaitNetSyncNode finishes correctly under client-only server wait synchronization policy.
        /// </summary>
        [Test]
        public void AbilityNodesTests_WaitNetSyncNode_FinishesOnClientOnlyServerWait()
        {
            var node = new WaitNetSyncNode();
            node.Initialise(_context);
            node.SyncType = AbilityNetSyncType.OnlyServerWait;

            SourceMock.Setup(m => m.IsLocalClient()).Returns(true);
            SourceMock.Setup(m => m.IsServer()).Returns(false);

            var replicationMock = new Mock<IReplicationManager>();
            SourceMock.Setup(m => m.ReplicationManager).Returns(replicationMock.Object);

            _abilityMock.Object.SetPredictionKey(new PredictionKey());

            var finished = false;
            node.onProcessFinished += (n) => finished = true;

            AbilityGraphTestUtilities.InvokeProcess(node);

            Assert.IsTrue(finished);
        }

        /// <summary>
        /// Validates that WaitTargetDataNode finishes execution when target data confirmation is received.
        /// </summary>
        [Test]
        public void AbilityNodesTests_WaitTargetDataNode_FinishesOnConfirm()
        {
            var node = new WaitTargetDataNode();
            node.Initialise(_context);

            SourceMock.Setup(m => m.IsLocalClient()).Returns(true);

            var finished = false;
            node.onProcessFinished += (n) => finished = true;

            AbilityGraphTestUtilities.InvokeProcess(node);

            var taskField = typeof(WaitTargetDataNode).GetField("_task", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var task = (WaitTargetDataTask)taskField.GetValue(node);
            Assert.IsNotNull(task);

            // Confirm target data
            var handle = new AbilitySystem.Runtime.Abilities.Targeting.TargetDataHandle();
            task.ConfirmTargetData(handle);

            Assert.IsTrue(finished);
        }
    }
}
