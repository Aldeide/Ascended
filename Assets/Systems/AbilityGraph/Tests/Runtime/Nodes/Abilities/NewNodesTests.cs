using AbilityGraph.Runtime.Nodes.Abilities;
using AbilityGraph.Runtime.Nodes.Logic;
using AbilityGraph.Runtime.Nodes.Math;
using AbilityGraph.Runtime.Nodes.Utilities;
using AbilityGraph.Runtime;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Abilities
{
    [TestFixture]
    public class NewNodesTests : AbilitySystemTestBase
    {
        private Mock<Ability> _abilityMock;
        private GraphContext _context;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _abilityMock = new Mock<Ability>();
            _abilityMock.Setup(a => a.Owner).Returns(Source);
            _abilityMock.Setup(a => a.Level).Returns(3);
            _context = new GraphContext(_abilityMock.Object, Source);
        }

        [Test]
        public void Test_AddTagToOwnerNode()
        {
            var node = new AddTagToOwnerNode();
            node.Initialise(_context);

            var tag = new Tag("Test.TagToAdd");
            node.Tag = tag;

            var tagManagerMock = new Mock<GameplayTagManager>(Source);
            SourceMock.Setup(m => m.TagManager).Returns(tagManagerMock.Object);

            AbilityGraphTestUtilities.InvokeProcess(node);

            tagManagerMock.Verify(m => m.AddTag(tag), Times.Once);
        }

        [Test]
        public void Test_RemoveTagFromOwnerNode()
        {
            var node = new RemoveTagFromOwnerNode();
            node.Initialise(_context);

            var tag = new Tag("Test.TagToRemove");
            node.Tag = tag;

            var tagManagerMock = new Mock<GameplayTagManager>(Source);
            SourceMock.Setup(m => m.TagManager).Returns(tagManagerMock.Object);

            AbilityGraphTestUtilities.InvokeProcess(node);

            tagManagerMock.Verify(m => m.RemoveTag(tag), Times.Once);
        }

        [Test]
        public void Test_ClampFloatNode()
        {
            var node = new ClampFloatNode();
            node.Initialise(_context);

            node.Value = 15f;
            node.Min = 0f;
            node.Max = 10f;

            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(10f, node.Result);

            node.Value = -5f;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(0f, node.Result);

            node.Value = 5f;
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.AreEqual(5f, node.Result);
        }

        [Test]
        public void Test_GetAttributePercentNode()
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

        [Test]
        public void Test_IsServerNode()
        {
            var node = new IsServerNode();
            node.Initialise(_context);

            SourceMock.Setup(m => m.IsServer()).Returns(true);
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsTrue(node.IsServer);

            SourceMock.Setup(m => m.IsServer()).Returns(false);
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsFalse(node.IsServer);
        }

        [Test]
        public void Test_IsLocalClientNode()
        {
            var node = new IsLocalClientNode();
            node.Initialise(_context);

            SourceMock.Setup(m => m.IsLocalClient()).Returns(true);
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsTrue(node.IsLocal);

            SourceMock.Setup(m => m.IsLocalClient()).Returns(false);
            AbilityGraphTestUtilities.InvokeProcess(node);
            Assert.IsFalse(node.IsLocal);
        }

        [Test]
        public void Test_DoOnceNode()
        {
            var node = new DoOnceNode();
            node.Initialise(_context);

            // Initially, executes once
            var nodes = node.GetExecutedNodes().ToList();
            Assert.IsNotNull(nodes);

            // Second time, it shouldn't trigger
            var nodesEmpty = node.GetExecutedNodes().ToList();
            Assert.AreEqual(0, nodesEmpty.Count);

            // Resetting trigger should allow it to run again
            node.ResetTrigger();
            var nodesAfterReset = node.GetExecutedNodes().ToList();
            Assert.IsNotNull(nodesAfterReset);
        }

        [Test]
        public void Test_SequenceNode()
        {
            var node = new SequenceNode();
            node.Initialise(_context);

            // Sequence should return executing links
            // We just verify it executes cleanly
            var executed = node.GetExecutedNodes();
            Assert.IsNotNull(executed);
        }

        [Test]
        public void Test_WaitInputPressNode()
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

        [Test]
        public void Test_WaitInputReleaseNode()
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

        [Test]
        public void Test_WaitNetSyncNode_ClientOnlyServerWait()
        {
            var node = new WaitNetSyncNode();
            node.Initialise(_context);
            node.SyncType = AbilityNetSyncType.OnlyServerWait;

            SourceMock.Setup(m => m.IsLocalClient()).Returns(true);
            SourceMock.Setup(m => m.IsServer()).Returns(false);

            var replicationMock = new Mock<IReplicationManager>();
            SourceMock.Setup(m => m.ReplicationManager).Returns(replicationMock.Object);

            var def = ScriptableObject.CreateInstance<AbilityDefinition>();
            def.UniqueName = "TestAbility";
            _abilityMock.Setup(a => a.Definition).Returns(def);
            _abilityMock.Setup(a => a.PredictionKey).Returns(new PredictionKey());

            var finished = false;
            node.onProcessFinished += (n) => finished = true;

            AbilityGraphTestUtilities.InvokeProcess(node);

            Assert.IsTrue(finished);
        }

        [Test]
        public void Test_WaitTargetDataNode_Confirm()
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
