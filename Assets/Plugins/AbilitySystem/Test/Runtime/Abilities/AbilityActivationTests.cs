using AbilitySystem.Runtime.Abilities.AbilityActivation;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;

using static AbilitySystem.Test.Utilities.AbilityUtilities;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilityActivationTests
    {
        [Test]
        public void AbilityActivationTests_EventActivation_ActivatesOnEvent()
        {
            var abilitySystem = CreateMockServerAbilitySystem().Object;
            var abilityDefinition = CreateInstantAbilityDefinition();
            var eventActivation = new OnEventActivation
            {
                ActivationEvent = new TestGameplayEventType()
            };
            abilityDefinition.AbilityActivation = eventActivation;
            abilitySystem.AbilityManager.GrantAbility(abilityDefinition);
            var eventManager = abilitySystem.EventManager;
            var eventArgs = new TestGameplayEventArgs();
            
            eventManager.TriggerEvent(new TestGameplayEvent(eventArgs));
            
            Assert.IsTrue(abilitySystem.AbilityManager.Abilities[abilityDefinition.UniqueName].IsActive);
        }
    }
}