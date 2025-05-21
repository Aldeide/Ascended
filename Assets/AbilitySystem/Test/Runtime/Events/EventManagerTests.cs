using AbilitySystem.Runtime.Events;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;

namespace AbilitySystem.Test.Runtime.Events
{
    public class EventManagerTests
    {
        [Test]
        public void EventManagerTests_TriggerEvent_EventReceived()
        {
            var eventManager = new EventManager();
            var eventReceived = false;
            eventManager.Subscribe<TestGameplayEventArgs>((_) => eventReceived = true);
            
            eventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));
            
            Assert.IsTrue(eventReceived);
        }
        
        [Test]
        public void EventManagerTests_TriggerEventWithAbilitySystem_EventReceived()
        {
            var mockAbilitySystem = CreateMockAbilitySystem().Object;
            var eventReceived = false;
            mockAbilitySystem.EventManager.Subscribe<TestGameplayEventArgs>((_) => eventReceived = true);
            
            mockAbilitySystem.EventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));
            
            Assert.IsTrue(eventReceived);
        }
    }
}