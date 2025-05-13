using AbilitySystem.Runtime.Abilities.Cooldowns;
using AbilitySystem.Runtime.Tags;
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
            var owner = CreateMockAbilitySystem();
            var abilityDefinition = CreateInstantAbilityDefinition();
            abilityDefinition.Cooldown = new ConstantAbilityCooldown();
            var cooldownTag = new GameplayTag("CooldownTag");
            abilityDefinition.Cooldown.CooldownTag = cooldownTag;
            abilityDefinition.Cooldown.BaseDuration = 10f;
            owner.Setup(m => m.IsServer()).Returns(true);
            owner.Object.AbilityManager.GrantAbility(abilityDefinition);
            
            owner.Object.AbilityManager.TryActivateAbility(abilityDefinition.uniqueName);
            
            Assert.AreEqual(10f, owner.Object.EffectManager.GetEffect(cooldownTag).Duration);
        }
    }
}