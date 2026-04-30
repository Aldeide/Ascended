using System;
using AbilityGraph.Runtime.Nodes.Abilities;
using AbilityGraph.Runtime.Nodes.Spatial;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AbilityGraph.Tests.Runtime.Integration
{
    public class AbilityGraphIntegrationTests
    {
        private Mock<IAbilitySystem> _clientSystem;
        private Mock<IAbilitySystem> _serverSystem;
        private GameObject _targetObj;
        private DummyAbilitySystemComponent _dummyTarget;
        private Mock<IAbilitySystem> _mockTargetSystem;
        private EffectDefinition _effectDef;

        [SetUp]
        public void Setup()
        {
            _clientSystem = new Mock<IAbilitySystem>();
            _clientSystem.Setup(x => x.IsServer()).Returns(false);
            _clientSystem.Setup(x => x.IsLocalClient()).Returns(true);

            _serverSystem = new Mock<IAbilitySystem>();
            _serverSystem.Setup(x => x.IsServer()).Returns(true);
            _serverSystem.Setup(x => x.IsLocalClient()).Returns(false);

            _targetObj = new GameObject("Target");
            _dummyTarget = _targetObj.AddComponent<DummyAbilitySystemComponent>();
            _mockTargetSystem = new Mock<IAbilitySystem>();
            _dummyTarget.MockSystem = _mockTargetSystem.Object;

            _effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_targetObj);
            Object.DestroyImmediate(_effectDef);
        }

        [Test]
        public void PredictedWeaponFiring_LogicSeparation()
        {
            // Scenario: A full-auto weapon firing.
            // Client predicts hitscan (Trace) and visual cues, but Damage (ApplyEffect) is ServerOnly.

            var applyNode = new ApplyEffectToTargetNode
            {
                Target = _targetObj,
                EffectDefinition = _effectDef,
                ServerOnly = true
            };

            // 1. Client Execution
            InjectOwner(applyNode, _clientSystem.Object);
            InvokeProcess(applyNode);

            // Client should NOT have applied the effect because it's ServerOnly
            _mockTargetSystem.Verify(x => x.ApplyEffectToSelf(It.IsAny<Effect>()), Times.Never);

            // 2. Server Execution
            InjectOwner(applyNode, _serverSystem.Object);
            _serverSystem.Setup(x => x.MakeOutgoingEffect(It.IsAny<EffectDefinition>(), It.IsAny<int>(), It.IsAny<EffectContext>()))
                .Returns(new Effect(_effectDef));

            InvokeProcess(applyNode);

            // Server SHOULD have applied the effect
            _mockTargetSystem.Verify(x => x.ApplyEffectToSelf(It.IsAny<Effect>()), Times.Once);
        }

        private void InjectOwner(object node, IAbilitySystem owner)
        {
            var field = typeof(AbilityGraph.Runtime.Nodes.Base.AbilityNode).GetField("Owner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(node, owner);
        }

        private void InvokeProcess(object node)
        {
            var method = node.GetType().GetMethod("Process", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(node, null);
        }
    }


}
