using AbilitySystem.Runtime.Abilities.AbilityActivation;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Abilities
{
    /// <summary>
    /// Unit tests for ability activation triggers, ensuring that abilities correctly respond to various initiation methods.
    /// </summary>
    public class AbilityActivationTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Verifies that an ability configured with an event-based activation trigger correctly activates when the specified gameplay event is fired.
        /// </summary>
        [Test]
        public void AbilityActivationTests_EventTrigger_ActivatesOnMatchingEvent()
        {
            var abilityDefinition = AbilityUtilities.CreateInstantAbilityDefinition();
            abilityDefinition.UniqueName = "EventAbility";
            var eventActivation = new OnEventActivation
            {
                ActivationEvent = new TestGameplayEventType()
            };
            abilityDefinition.AbilityActivation = eventActivation;
            
            Source.AbilityManager.GrantAbility(abilityDefinition);
            
            // Trigger the event through the system's event manager
            var eventArgs = new TestGameplayEventArgs();
            Source.EventManager.TriggerEvent(new TestGameplayEvent(eventArgs));
            
            Assert.IsTrue(Source.AbilityManager.Abilities["EventAbility"].IsActive, "Ability should have activated in response to the gameplay event");
        }
    }
}