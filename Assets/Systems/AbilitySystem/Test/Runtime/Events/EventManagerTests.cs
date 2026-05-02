using System;
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

        [Test]
        public void EventManagerTests_GameplayEvent_ArgumentsCanBeRetrieved()
        {
            var args = new TestGameplayEventArgs();
            var gameEvent = new TestGameplayEvent(args);
            
            Assert.AreEqual(args, gameEvent.Arguments);
        }

        [Test]
        public void EventManagerTests_SubscribeTwice_BothHandlersCalled()
        {
            var eventManager = new EventManager();
            int callCount = 0;
            Action<GameplayEvent> handler = e => callCount++;
            
            eventManager.Subscribe(typeof(TestGameplayEvent), handler);
            eventManager.Subscribe(typeof(TestGameplayEvent), handler);
            
            eventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));
            
            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void EventManagerTests_UnsubscribeNotSubscribed_DoesNotThrow()
        {
            var eventManager = new EventManager();
            Action<GameplayEvent> handler = e => { };
            
            Assert.DoesNotThrow(() => eventManager.Unsubscribe(typeof(TestGameplayEvent), handler));
        }

        [Test]
        public void EventManagerTests_TriggerEventNoSubscribers_DoesNotThrow()
        {
            var eventManager = new EventManager();
            Assert.DoesNotThrow(() => eventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs())));
        }

        private void TestEventHandler(GameplayEvent gameplayEvent)
        {
            _eventReceived = true;
        }
    }
}