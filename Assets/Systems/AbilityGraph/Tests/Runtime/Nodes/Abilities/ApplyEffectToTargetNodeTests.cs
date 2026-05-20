using AbilityGraph.Runtime;
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
    /// <summary>
    /// Tests for the ApplyEffectToTargetNode, focusing on server-only logic and cross-system effect application.
    /// </summary>
    public class ApplyEffectToTargetNodeTests : AbilitySystemTestBase
    {
        private GameObject _targetObj;
        private Mock<IAbilitySystem> _mockTargetSystem;
        private ApplyEffectToTargetNode _node;
        private EffectDefinition _effectDef;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _targetObj = new GameObject("Target");
            // Since our system relies on GetComponents in Unity, we need a MonoBehaviour that implements IAbilitySystem.
            _targetObj.AddComponent<DummyAbilitySystemComponent>();
            var dummyTarget = _targetObj.GetComponent<DummyAbilitySystemComponent>();

            _mockTargetSystem = new Mock<IAbilitySystem>();
            dummyTarget.MockSystem = _mockTargetSystem.Object;

            _effectDef = ScriptableObject.CreateInstance<EffectDefinition>();

            _node = new ApplyEffectToTargetNode
            {
                Target = _targetObj,
                EffectDefinition = _effectDef,
                Level = 1,
                ServerOnly = true
            };

            var abilityMock = new Mock<Ability>();
            var context = new GraphContext(abilityMock.Object, Source);
            _node.Initialise(context);
        }

        [TearDown]
        public void Teardown()
        {
            if (_targetObj != null) Object.DestroyImmediate(_targetObj);
            if (_effectDef != null) Object.DestroyImmediate(_effectDef);
        }

        /// <summary>
        /// Validates that an ApplyEffect node with ServerOnly=true does NOT apply the effect when running on a client.
        /// </summary>
        [Test]
        public void ApplyEffectToTargetNodeTests_Process_ServerOnlyTrueClientOwner_DoesNotApplyEffect()
        {
            SourceMock.Setup(x => x.IsServer()).Returns(false);
            
            AbilityGraphTestUtilities.InvokeProcess(_node);

            _mockTargetSystem.Verify(x => x.ApplyEffectToSelf(It.IsAny<Effect>()), Times.Never);
        }

        /// <summary>
        /// Validates that an ApplyEffect node with ServerOnly=true DOES apply the effect when running on the server.
        /// </summary>
        [Test]
        public void ApplyEffectToTargetNodeTests_Process_ServerOnlyTrueServerOwner_AppliesEffectToTarget()
        {
            SourceMock.Setup(x => x.IsServer()).Returns(true);
            SourceMock.Setup(x => x.MakeOutgoingEffect(It.IsAny<EffectDefinition>(), It.IsAny<int>(), It.IsAny<EffectContext>()))
                .Returns(new Effect(_effectDef));
            
            AbilityGraphTestUtilities.InvokeProcess(_node);

            _mockTargetSystem.Verify(x => x.ApplyEffectToSelf(It.IsAny<Effect>()), Times.Once);
        }
    }
}
