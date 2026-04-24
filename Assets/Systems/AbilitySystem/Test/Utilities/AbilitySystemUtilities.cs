using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Runtime.Tags;
using Moq;

namespace AbilitySystem.Test.Utilities
{
    public static class AbilitySystemUtilities
    {
        public static Mock<IAbilitySystem> CreateMockAbilitySystem()
        {
            var owner = new Mock<IAbilitySystem>();
            SetupDefaultMocks(owner);
            return owner;
        }
        
        public static Mock<IAbilitySystem> CreateMockServerAbilitySystem()
        {
            var owner = new Mock<IAbilitySystem>();
            owner.Setup(x => x.IsServer()).Returns(true);
            owner.Setup(x => x.IsLocalClient()).Returns(false);
            SetupDefaultMocks(owner);
            return owner;
        }
        
        public static Mock<IAbilitySystem> CreateMockClientAbilitySystem()
        {
            var owner = new Mock<IAbilitySystem>();
            owner.Setup(x => x.IsServer()).Returns(false);
            owner.Setup(x=>x.IsHost()).Returns(false);
            owner.Setup(x => x.IsLocalClient()).Returns(true);
            SetupDefaultMocks(owner);
            return owner;
        }

        private static void SetupDefaultMocks(Mock<IAbilitySystem> owner)
        {
            var effectManager = new EffectManager(owner.Object);
            owner.Setup(x => x.EffectManager).Returns(effectManager);
            var eventManager = new EventManager();
            owner.Setup(x => x.EventManager).Returns(eventManager);
            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(x => x.TagManager).Returns(tagManager);
            var attributeSetManager = new AttributeSetManager(owner.Object);
            attributeSetManager.AddAttributeSet(typeof(TestAttributeSet), new TestAttributeSet(owner.Object));
            owner.SetupGet(x => x.AttributeSetManager).Returns(attributeSetManager);
            var replicationManager = new MockReplicationManager(owner.Object);
            owner.Setup(x => x.ReplicationManager).Returns(replicationManager);
            var abilityManager = new AbilityManager(owner.Object);
            owner.Setup(x => x.AbilityManager).Returns(abilityManager);
            var dataManager = new Mock<IDataManager>();
            owner.Setup(x => x.DataManager).Returns(dataManager.Object);
            replicationManager.DataManager = dataManager.Object;

            owner.Setup(x => x.MakeEffectContext()).Returns(() => new EffectContext(owner.Object, owner.Object));
            owner.Setup(x => x.MakeOutgoingEffect(It.IsAny<EffectDefinition>(), It.IsAny<int>(), It.IsAny<EffectContext>()))
                .Returns((EffectDefinition def, int level, EffectContext context) => def.ToEffect(owner.Object, owner.Object, context));
            owner.Setup(x => x.ApplyEffectToSelf(It.IsAny<Effect>()))
                .Returns((Effect eff) =>
                {
                    eff.Activate();
                    return owner.Object.EffectManager.AddEffect(eff);
                });
        }
    }
}