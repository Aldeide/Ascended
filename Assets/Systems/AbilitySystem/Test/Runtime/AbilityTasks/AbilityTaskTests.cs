using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.AbilityTasks
{
    /// <summary>
    /// Unit tests for AbilityTasks, verifying lifecycle management, event-driven completion, and automatic cleanup.
    /// </summary>
    public class AbilityTaskTests : AbilitySystemTestBase
    {
        private TestAbility _ability;
        private AbilityDefinition _abilityDefinition;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            _abilityDefinition = ScriptableObject.CreateInstance<LocalTestAbilityDefinition>();
            _ability = (TestAbility)_abilityDefinition.ToAbility(Source);
            
            // Force ability to be active for task execution
            _ability.IsActive = true;
        }

        /// <summary>
        /// Verifies that WaitDelayTask correctly identifies as finished after the specified duration has elapsed.
        /// </summary>
        [Test]
        public void AbilityTaskTests_WaitDelay_CompletesAfterSpecifiedDuration()
        {
            float currentTime = 0f;
            SourceMock.Setup(x => x.GetTime()).Returns(() => currentTime);

            bool isFinished = false;
            var task = WaitDelayTask.CreateWaitDelay(_ability, 1.0f);
            task.OnFinished += () => isFinished = true;
            task.ReadyForActivation();

            Assert.IsFalse(isFinished, "Task should not be finished initially");
            
            currentTime = 0.5f;
            _ability.Tick(); 
            Assert.IsFalse(isFinished, "Task should not be finished halfway through the delay");

            currentTime = 1.1f;
            _ability.Tick();
            Assert.IsTrue(isFinished, "Task should be finished after duration has passed");
            Assert.IsFalse(task.IsActive, "Task should no longer be active after finishing");
        }

        /// <summary>
        /// Verifies that WaitGameplayEventTask correctly triggers its callback when a matching event is fired via the EventManager.
        /// </summary>
        [Test]
        public void AbilityTaskTests_WaitGameplayEvent_TriggersOnMatchingEvent()
        {
            bool eventFired = false;
            var task = WaitGameplayEventTask.CreateWaitGameplayEvent(_ability, typeof(TestGameplayEvent));
            task.OnEventReceived += (payload) => eventFired = true;
            task.ReadyForActivation();

            Assert.IsFalse(eventFired, "Event callback should not have fired yet");

            Source.EventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));

            Assert.IsTrue(eventFired, "Event callback should have fired after triggering the event");
            Assert.IsTrue(task.IsActive, "WaitGameplayEventTask should remain active by default for multiple events");
        }

        /// <summary>
        /// Verifies that all active tasks associated with an ability are automatically terminated when the ability is cancelled.
        /// </summary>
        [Test]
        public void AbilityTaskTests_AbilityCancel_AutomaticallyCleansUpTasks()
        {
            var task = WaitDelayTask.CreateWaitDelay(_ability, 10f);
            task.ReadyForActivation();

            Assert.IsTrue(task.IsActive);

            _ability.TryCancelAbility();

            Assert.IsFalse(task.IsActive, "Task should have been deactivated when ability was cancelled");
        }
        
        /// <summary>
        /// Verifies that tasks correctly unsubscribe from events and clean up when their parent ability ends.
        /// </summary>
        [Test]
        public void AbilityTaskTests_AbilityEnd_SuccessfullyCleansUpTasksAndSubscriptions()
        {
            var task = WaitGameplayEventTask.CreateWaitGameplayEvent(_ability, typeof(TestGameplayEvent));
            task.ReadyForActivation();

            Assert.IsTrue(task.IsActive);

            _ability.TryEndAbility();

            Assert.IsFalse(task.IsActive, "Task should no longer be active after ability ends");
            
            bool eventFired = false;
            task.OnEventReceived += (e) => eventFired = true;
            Source.EventManager.TriggerEvent(new TestGameplayEvent(new TestGameplayEventArgs()));
            
            Assert.IsFalse(eventFired, "Task should have been unsubscribed from the event manager");
        }

        #region Helper Classes
        private class TestAbility : Ability
        {
            public TestAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner) { }
            protected override void ActivateAbility(AbilityData data) { }
            public override void EndAbility() { }
        }

        private class LocalTestAbilityDefinition : AbilityDefinition
        {
            public override Type AbilityType() => typeof(TestAbility);
            public override Ability ToAbility(IAbilitySystem owner) => new TestAbility(this, owner);
        }
        #endregion
    }
}
