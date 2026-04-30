using AbilityGraph.Runtime.Nodes.Abilities;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
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
            _targetObj.AddComponent<DummyAbilitySystem>();
            var dummyTarget = _targetObj.GetComponent<DummyAbilitySystem>();

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

    public class DummyAbilitySystem : MonoBehaviour, IAbilitySystem
    {
        public IAbilitySystem MockSystem;

        public AbilitySystem.Runtime.Networking.INetworkRole NetworkRole { get => MockSystem.NetworkRole; set => MockSystem.NetworkRole = value; }
        public AbilitySystem.Runtime.Tags.GameplayTagManager TagManager { get => MockSystem.TagManager; set => MockSystem.TagManager = value; }
        public EffectManager EffectManager { get => MockSystem.EffectManager; set => MockSystem.EffectManager = value; }
        public AbilityManager AbilityManager { get => MockSystem.AbilityManager; set => MockSystem.AbilityManager = value; }
        public AbilitySystem.Runtime.AttributeSets.AttributeSetManager AttributeSetManager { get => MockSystem.AttributeSetManager; set => MockSystem.AttributeSetManager = value; }
        public AbilitySystem.Runtime.Cues.CueManager CueManager { get => MockSystem.CueManager; set => MockSystem.CueManager = value; }
        public AbilitySystem.Runtime.Networking.IReplicationManager ReplicationManager { get => MockSystem.ReplicationManager; set => MockSystem.ReplicationManager = value; }
        public AbilitySystem.Runtime.Core.IDataManager DataManager { get => MockSystem.DataManager; set => MockSystem.DataManager = value; }
        public AbilitySystem.Runtime.Events.EventManager EventManager { get => MockSystem.EventManager; set => MockSystem.EventManager = value; }

        public void Tick() => MockSystem.Tick();
        public float GetTime() => MockSystem.GetTime();
        public bool IsLocalClient() => MockSystem.IsLocalClient();
        public bool IsServer() => MockSystem.IsServer();
        public bool IsHost() => MockSystem.IsHost();
        public bool HasAuthority() => MockSystem.HasAuthority();
        public void PlayCue(AbilitySystem.Runtime.Cues.CueDefinition cue, bool isPredicted = false) => MockSystem.PlayCue(cue, isPredicted);
        public void PlayCue(AbilitySystem.Runtime.Cues.CueDefinition cue, AbilitySystem.Runtime.Cues.CueData data, bool isPredicted = false) => MockSystem.PlayCue(cue, data, isPredicted);
        public void PlayCue(string cueTag, AbilitySystem.Runtime.Cues.CueData data, bool isPredicted = false) => MockSystem.PlayCue(cueTag, data, isPredicted);
        public void PlayCue(GameplayTags.Runtime.Tag cueTag, AbilitySystem.Runtime.Cues.CueData data, bool isPredicted) => MockSystem.PlayCue(cueTag, data, isPredicted);
        public Effect MakeOutgoingEffect(EffectDefinition definition, int level = 1, EffectContext context = null) => MockSystem.MakeOutgoingEffect(definition, level, context);
        public EffectContext MakeEffectContext() => MockSystem.MakeEffectContext();
        public EffectApplicationResult ApplyEffectToSelf(Effect effect) => MockSystem.ApplyEffectToSelf(effect);
        public void Reset() => MockSystem?.Reset();
    }
}
