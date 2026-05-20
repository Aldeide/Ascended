using System;
using AbilityGraph.Runtime;
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
    /// <summary>
    /// Integration tests for the Ability Graph system, focusing on cross-node logic and network role separation.
    /// </summary>
    public class AbilityGraphIntegrationTests : AbilitySystemTestBase
    {
        private GameObject _targetObj;
        private DummyAbilitySystemComponent _dummyTarget;
        private Mock<IAbilitySystem> _mockTargetSystem;
        private EffectDefinition _effectDef;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _targetObj = new GameObject("Target");
            _dummyTarget = _targetObj.AddComponent<DummyAbilitySystemComponent>();
            _mockTargetSystem = new Mock<IAbilitySystem>();
            _dummyTarget.MockSystem = _mockTargetSystem.Object;

            _effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
        }

        [TearDown]
        public void Teardown()
        {
            if (_targetObj != null) Object.DestroyImmediate(_targetObj);
            if (_effectDef != null) Object.DestroyImmediate(_effectDef);
        }

        /// <summary>
        /// Validates that an ApplyEffect node with ServerOnly flag set correctly skips execution on clients
        /// and applies the effect on the server.
        /// </summary>
        [Test]
        public void AbilityGraphIntegrationTests_PredictedWeaponFiring_SeparatesClientAndServerLogic()
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
            var clientSystem = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            var clientAbility = new Mock<Ability>();
            applyNode.Initialise(new GraphContext(clientAbility.Object, clientSystem.Object));
            AbilityGraphTestUtilities.InvokeProcess(applyNode);

            // Client should NOT have applied the effect because it's ServerOnly
            _mockTargetSystem.Verify(x => x.ApplyEffectToSelf(It.IsAny<Effect>()), Times.Never);

            // 2. Server Execution
            var serverSystem = AbilitySystemUtilities.CreateMockServerAbilitySystem();
            var serverAbility = new Mock<Ability>();
            applyNode.Initialise(new GraphContext(serverAbility.Object, serverSystem.Object));
            
            // Server MUST be able to make the effect
            serverSystem.Setup(x => x.MakeOutgoingEffect(It.IsAny<EffectDefinition>(), It.IsAny<int>(), It.IsAny<EffectContext>()))
                .Returns(new Effect(_effectDef));

            AbilityGraphTestUtilities.InvokeProcess(applyNode);

            // Server SHOULD have applied the effect
            _mockTargetSystem.Verify(x => x.ApplyEffectToSelf(It.IsAny<Effect>()), Times.Once);
        }
    }
}
