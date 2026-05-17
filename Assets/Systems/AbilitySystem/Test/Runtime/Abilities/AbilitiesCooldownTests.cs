using AbilitySystem.Runtime.Abilities.Cooldowns;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Abilities
{
    /// <summary>
    /// Unit tests for ability cooldowns, ensuring that abilities correctly trigger, respect, and recover from cooldown effects.
    /// </summary>
    public class AbilitiesCooldownTests : AbilitySystemTestBase
    {
        private static readonly Tag CooldownTag = new Tag("CooldownTag");

        /// <summary>
        /// Verifies that activating an ability with a configured cooldown correctly applies the cooldown effect to the owner.
        /// </summary>
        [Test]
        public void AbilitiesCooldownTests_ActivateWithCooldown_TriggersCooldownEffect()
        {
            SetupAbilityWithCooldown();
            
            bool success = Source.AbilityManager.TryActivateAbility("TestAbility");
            
            Assert.IsTrue(success);
            Assert.IsNotNull(Source.EffectManager.GetEffect(CooldownTag));
            Assert.AreEqual(100f, Source.EffectManager.GetEffect(CooldownTag).Duration);
        }
        
        /// <summary>
        /// Verifies that an ability cannot be activated again if its cooldown effect is still active.
        /// </summary>
        [Test]
        public void AbilitiesCooldownTests_OnCooldown_BlockedFromActivation()
        {
            SetupAbilityWithCooldown();
            Source.AbilityManager.TryActivateAbility("TestAbility");
            Source.AbilityManager.EndAbility("TestAbility");
            
            bool secondActivation = Source.AbilityManager.TryActivateAbility("TestAbility");
            
            Assert.IsFalse(secondActivation, "Ability should be blocked by active cooldown");
        }
        
        /// <summary>
        /// Verifies that an ability becomes available for activation once the system time exceeds the cooldown duration.
        /// </summary>
        [Test]
        public void AbilitiesCooldownTests_CooldownElapsed_AllowsReactivation()
        {
            SetupAbilityWithCooldown();
            Source.AbilityManager.TryActivateAbility("TestAbility");
            Source.AbilityManager.EndAbility("TestAbility");
            
            // Advance time beyond the 100s cooldown
            SourceMock.Setup(m => m.GetTime()).Returns(101f);
            Source.EffectManager.Tick();
            
            bool reactivation = Source.AbilityManager.TryActivateAbility("TestAbility");
            
            Assert.IsTrue(reactivation, "Ability should be available after cooldown expires");
        }

        private void SetupAbilityWithCooldown()
        {
            var abilityDefinition = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDefinition.UniqueName = "TestAbility";
            abilityDefinition.Cooldown = new ConstantAbilityCooldown();
            
            var cooldownEffect = EffectUtilities.CreateDurationEffectDefinition();
            cooldownEffect.AssetTags = new[] { CooldownTag };
            cooldownEffect.GrantedTags = new[] { CooldownTag };
            abilityDefinition.Cooldown.CooldownEffect = cooldownEffect;
            
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            Source.AbilityManager.GrantAbility(abilityDefinition);
        }
    }
}