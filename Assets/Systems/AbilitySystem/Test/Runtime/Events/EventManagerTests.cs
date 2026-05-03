using System;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Events
{
    /// <summary>
    /// Unit tests for the EventManager, verifying correct subscription, unsubscription, and event propagation.
    /// </summary>
    public class EventManagerTests : AbilitySystemTestBase
    {
        private bool _eventReceived;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _eventReceived = false;
        }

        /// <summary>
        /// Verifies that subscribing to a specific gameplay event type correctly results in the handler being called when that event is triggered.
        /// </summary>
        [Test]
        public void EventManagerTests_TriggerEvent_HandlerIsCalled()
        {
            var eventManager = new EventManager();
            eventManager.Subscribe(typeof(TestGameplayEvent), TestEventHandler);
            
            eventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));
            
            Assert.IsTrue(_eventReceived, "Event handler should have been invoked");
        }
        
        /// <summary>
        /// Verifies that the EventManager integrated within an AbilitySystem instance correctly handles event distribution.
        /// </summary>
        [Test]
        public void EventManagerTests_AbilitySystemTrigger_HandlerIsCalled()
        {
            Source.EventManager.Subscribe(typeof(TestGameplayEvent), TestEventHandler);
            
            Source.EventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));
            
            Assert.IsTrue(_eventReceived, "Event handler subscribed via AbilitySystem should have been invoked");
        }
        
        /// <summary>
        /// Verifies that unsubscribing from an event type correctly prevents further calls to the handler.
        /// </summary>
        [Test]
        public void EventManagerTests_Unsubscribe_PreventsFurtherHandlerCalls()
        {
            Source.EventManager.Subscribe(typeof(TestGameplayEvent), TestEventHandler);
            Source.EventManager.Unsubscribe(typeof(TestGameplayEvent), TestEventHandler);
            
            Source.EventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));
            
            Assert.IsFalse(_eventReceived, "Event handler should not have been invoked after unsubscription");
        }

        /// <summary>
        /// Verifies that a GameplayEvent instance correctly preserves and returns its associated arguments.
        /// </summary>
        [Test]
        public void EventManagerTests_GameplayEvent_CorrectlyStoresArguments()
        {
            var args = new TestGameplayEventArgs();
            var gameEvent = new TestGameplayEvent(args);
            
            Assert.AreEqual(args, gameEvent.Arguments);
        }

        /// <summary>
        /// Verifies that subscribing the same handler multiple times results in it being called multiple times upon triggering the event.
        /// </summary>
        [Test]
        public void EventManagerTests_SubscribeTwice_HandlerIsCalledMultipleTimes()
        {
            var eventManager = new EventManager();
            int callCount = 0;
            Action<GameplayEvent> handler = e => callCount++;
            
            eventManager.Subscribe(typeof(TestGameplayEvent), handler);
            eventManager.Subscribe(typeof(TestGameplayEvent), handler);
            
            eventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));
            
            Assert.AreEqual(2, callCount, "Handler should have been called twice for two subscriptions");
        }

        /// <summary>
        /// Verifies that attempting to unsubscribe a handler that was never subscribed does not cause an exception.
        /// </summary>
        [Test]
        public void EventManagerTests_UnsubscribeNotSubscribed_DoesNotThrow()
        {
            var eventManager = new EventManager();
            Action<GameplayEvent> handler = e => { };
            
            Assert.DoesNotThrow(() => eventManager.Unsubscribe(typeof(TestGameplayEvent), handler));
        }

        /// <summary>
        /// Verifies that triggering an event for which there are no subscribers does not cause an exception.
        /// </summary>
        [Test]
        public void EventManagerTests_TriggerNoSubscribers_DoesNotThrow()
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