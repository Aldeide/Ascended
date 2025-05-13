using AbilitySystem.Runtime.Abilities.Cooldowns;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;
using static AbilitySystem.Test.Utilities.AbilityUtilities;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilitiesCooldownTests
    {
        [Test]
        public void AbilitiesCooldownTests_HasCooldown_TriggersCooldown()
        {
            var owner = SetupAbilitySystemWithAbilityCooldown();
            owner.Object.AbilityManager.TryActivateAbility("TestAbility");
            
            Assert.AreEqual(100f, owner.Object.EffectManager.GetEffect(new GameplayTag("CooldownTag")).Duration);
        }
        
        [Test]
        public void AbilitiesCooldownTests_OnCooldown_CantBeActivated()
        {
            var owner = SetupAbilitySystemWithAbilityCooldown();
            owner.Object.AbilityManager.TryActivateAbility("TestAbility");
            
            Assert.IsFalse(owner.Object.AbilityManager.TryActivateAbility("TestAbility"));
        }
        
        [Test]
        public void AbilitiesCooldownTests_CooldownElapsed_CanBeActivated()
        {
            var owner = SetupAbilitySystemWithAbilityCooldown();
            owner.Object.AbilityManager.TryActivateAbility("TestAbility");
            owner.Setup(m => m.GetTime()).Returns(101);
            owner.Object.EffectManager.Tick();
            
            Assert.IsTrue(owner.Object.AbilityManager.TryActivateAbility("TestAbility"));
        }

        private Mock<IAbilitySystem> SetupAbilitySystemWithAbilityCooldown()
        {
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.Cooldown = new ConstantAbilityCooldown();
            var cooldownEffect = EffectUtilities.CreateDurationEffectDefinition();
            var cooldownTag = new GameplayTag("CooldownTag");
            cooldownEffect.assetTags = new[] { cooldownTag };
            cooldownEffect.grantedTags = new[] { cooldownTag };
            abilityDefinition.Cooldown.CooldownEffect = cooldownEffect;
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            return owner;
        }
    }
}