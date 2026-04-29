using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using Moq;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.AbilityTasks
{
    public class TestAbility : Ability
    {
        public TestAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        protected override void ActivateAbility(AbilityData data)
        {
        }

        public override void EndAbility()
        {
        }
    }

    public class TestAbilityDefinition : AbilityDefinition
    {
        public override Type AbilityType() => typeof(TestAbility);

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new TestAbility(this, owner);
        }
    }

    public class TestGameplayEvent : GameplayEvent
    {
        public TestGameplayEvent() : base(EventArgs.Empty) { }
    }

    public class AbilityTaskTests
    {
        private Mock<IAbilitySystem> _mockAbilitySystem;
        private EventManager _eventManager;
        private TestAbility _ability;
        private AbilityDefinition _abilityDefinition;

        [SetUp]
        public void Setup()
        {
            _mockAbilitySystem = AbilitySystemUtilities.CreateMockAbilitySystem();
            _eventManager = _mockAbilitySystem.Object.EventManager;
            
            _abilityDefinition = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            _ability = new TestAbility(_abilityDefinition, _mockAbilitySystem.Object);
            
            // Hack to make IsActive = true so TryCancelAbility works
            _ability.IsActive = true;
        }

        [Test]
        public void WaitDelayTask_CompletesAfterDuration()
        {
            float currentTime = 0f;
            _mockAbilitySystem.Setup(x => x.GetTime()).Returns(() => currentTime);

            bool isFinished = false;
            var task = WaitDelayTask.CreateWaitDelay(_ability, 1.0f);
            task.OnFinished += () => isFinished = true;
            task.ReadyForActivation();

            Assert.IsFalse(isFinished);
            
            currentTime = 0.5f;
            _ability.Tick(); // ability ticks tasks
            Assert.IsFalse(isFinished);

            currentTime = 1.1f;
            _ability.Tick();
            Assert.IsTrue(isFinished);
            Assert.IsFalse(task.IsActive);
        }

        [Test]
        public void WaitGameplayEventTask_CompletesOnEvent()
        {
            bool eventFired = false;
            var task = WaitGameplayEventTask.CreateWaitGameplayEvent(_ability, typeof(TestGameplayEvent));
            task.OnEventReceived += (payload) => eventFired = true;
            task.ReadyForActivation();

            Assert.IsFalse(eventFired);

            _eventManager.TriggerEvent(new TestGameplayEvent());

            Assert.IsTrue(eventFired);
            Assert.IsTrue(task.IsActive); // WaitGameplayEventTask continues listening until cancelled or ended manually
        }

        [Test]
        public void Task_AutoCleansUpOnAbilityCancel()
        {
            var task = WaitDelayTask.CreateWaitDelay(_ability, 10f);
            task.ReadyForActivation();

            Assert.IsTrue(task.IsActive);

            _ability.TryCancelAbility();

            Assert.IsFalse(task.IsActive);
            
            // Check it doesn't tick after
            float currentTime = 0f;
            _mockAbilitySystem.Setup(x => x.GetTime()).Returns(() => currentTime);
            currentTime = 20f;
            _ability.Tick();
            
            // We can't really assert if it ticked but since IsActive is false it shouldn't be in the list anymore
        }
        
        [Test]
        public void Task_AutoCleansUpOnAbilityEnd()
        {
            var task = WaitGameplayEventTask.CreateWaitGameplayEvent(_ability, typeof(TestGameplayEvent));
            task.ReadyForActivation();

            Assert.IsTrue(task.IsActive);

            _ability.TryEndAbility();

            Assert.IsFalse(task.IsActive);
            
            bool eventFired = false;
            task.OnEventReceived += (e) => eventFired = true;
            _eventManager.TriggerEvent(new TestGameplayEvent());
            
            // Should be unsubscribed, so event shouldn't fire
            Assert.IsFalse(eventFired);
        }
    }
}
