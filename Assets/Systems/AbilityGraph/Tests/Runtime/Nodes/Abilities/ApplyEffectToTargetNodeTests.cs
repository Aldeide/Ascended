using AbilityGraph.Runtime.Nodes.Abilities;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace AbilityGraph.Tests.Runtime.Nodes.Abilities
{
    public class ApplyEffectToTargetNodeTests
    {
        private GameObject _targetObj;
        private Mock<IAbilitySystem> _mockTargetSystem;
        private Mock<IAbilitySystem> _mockSourceSystem;
        private ApplyEffectToTargetNode _node;
        private EffectDefinition _effectDef;

        [SetUp]
        public void Setup()
        {
            _targetObj = new GameObject("Target");
            // Since our system relies on GetComponents in Unity, we need a MonoBehaviour that implements IAbilitySystem.
            // For testing, we can use an empty monobehaviour and manually mock the node's Process by mocking Owner.
            // Wait, the node uses Target.GetComponent<IAbilitySystem>(). This requires a real MonoBehaviour.
            // Let's create a dummy component.
            _targetObj.AddComponent<DummyAbilitySystemComponent>();
            var dummyTarget = _targetObj.GetComponent<DummyAbilitySystemComponent>();

            _mockTargetSystem = new Mock<IAbilitySystem>();
            dummyTarget.MockSystem = _mockTargetSystem.Object;

            _mockSourceSystem = new Mock<IAbilitySystem>();
            
            _effectDef = ScriptableObject.CreateInstance<EffectDefinition>();

            _node = new ApplyEffectToTargetNode
            {
                Target = _targetObj,
                EffectDefinition = _effectDef,
                Level = 1,
                ServerOnly = true
            };
            
            var method = typeof(ApplyEffectToTargetNode).GetProperty("Owner", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            // We can't set Owner directly because it's pulled from the Graph.
            // We need to initialize the node via reflection or inject Owner.
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_targetObj);
            Object.DestroyImmediate(_effectDef);
        }

        [Test]
        public void Process_ServerOnlyTrue_ClientOwner_DoesNotApplyEffect()
        {
            _mockSourceSystem.Setup(x => x.IsServer()).Returns(false);
            
            var field = typeof(AbilityGraph.Runtime.Nodes.Base.AbilityNode).GetField("Owner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_node, _mockSourceSystem.Object);

            var method = typeof(ApplyEffectToTargetNode).GetMethod("Process", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(_node, null);

            _mockTargetSystem.Verify(x => x.ApplyEffectToSelf(It.IsAny<Effect>()), Times.Never);
        }

        [Test]
        public void Process_ServerOnlyTrue_ServerOwner_AppliesEffect()
        {
            _mockSourceSystem.Setup(x => x.IsServer()).Returns(true);
            _mockSourceSystem.Setup(x => x.MakeOutgoingEffect(It.IsAny<EffectDefinition>(), It.IsAny<int>(), It.IsAny<EffectContext>()))
                .Returns(new Effect(_effectDef));
            
            var field = typeof(AbilityGraph.Runtime.Nodes.Base.AbilityNode).GetField("Owner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_node, _mockSourceSystem.Object);

            var method = typeof(ApplyEffectToTargetNode).GetMethod("Process", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(_node, null);

            _mockTargetSystem.Verify(x => x.ApplyEffectToSelf(It.IsAny<Effect>()), Times.Once);
        }
    }


}
