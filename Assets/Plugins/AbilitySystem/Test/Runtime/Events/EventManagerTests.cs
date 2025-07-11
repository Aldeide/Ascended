using AbilitySystem.Runtime.Events;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using static AbilitySystem.Test.Utilities.AbilitySystemUtilities;

namespace AbilitySystem.Test.Runtime.Events
{
    public class EventManagerTests
    {
        private bool _eventReceived;

        [TearDown]
        public void TearDown()
        {
            _eventReceived = false;
        }
        
        [Test]
        public void EventManagerTests_TriggerEvent_EventReceived()
        {
            var eventManager = new EventManager();
            eventManager.Subscribe(typeof(TestGameplayEvent), TestEventHandler);
            
            eventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));
            
            Assert.IsTrue(_eventReceived);
        }
        
        [Test]
        public void EventManagerTests_TriggerEventWithAbilitySystem_EventReceived()
        {
            var mockAbilitySystem = CreateMockAbilitySystem().Object;
            mockAbilitySystem.EventManager.Subscribe(typeof(TestGameplayEvent), TestEventHandler);
            
            mockAbilitySystem.EventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));
            
            Assert.IsTrue(_eventReceived);
        }
        
        [Test]
        public void EventManagerTests_Unsubscribe_DoesNotReceiveEvent()
        {
            var mockAbilitySystem = CreateMockAbilitySystem().Object;
            mockAbilitySystem.EventManager.Subscribe(typeof(TestGameplayEvent), TestEventHandler);
            mockAbilitySystem.EventManager.Unsubscribe(typeof(TestGameplayEvent), TestEventHandler);
            
            mockAbilitySystem.EventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));
            
            Assert.IsFalse(_eventReceived);
        }

        private void TestEventHandler(GameplayEvent gameplayEvent)
        {
            _eventReceived = true;
        }
    }
}