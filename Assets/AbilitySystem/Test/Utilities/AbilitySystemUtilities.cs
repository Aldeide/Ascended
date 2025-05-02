using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using Moq;

namespace AbilitySystem.Test.Utilities
{
    public static class AbilitySystemUtilities
    {
        public static Mock<IAbilitySystem> CreateMockAbilitySystem()
        {
            var owner = new Mock<IAbilitySystem>();
            var effectManager = new EffectManager(owner.Object);
            owner.Setup(x => x.EffectManager).Returns(effectManager);
            var tagManager = new GameplayTagManager(owner.Object);
            owner.Setup(x => x.TagManager).Returns(tagManager);
            var attributeSetManager = new AttributeSetManager(owner.Object);
            attributeSetManager.AddAttributeSet(typeof(TestAttributeSet), new TestAttributeSet(owner.Object));
            owner.SetupGet(x => x.AttributeSetManager).Returns(attributeSetManager);
            var abilityManager = new AbilityManager(owner.Object);
            owner.Setup(x => x.AbilityManager).Returns(abilityManager);
            return owner;
        }
    }
}