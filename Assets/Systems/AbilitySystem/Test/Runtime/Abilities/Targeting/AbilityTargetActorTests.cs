using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.Targeting;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AbilitySystem.Test.Runtime.Abilities.Targeting
{
    public class MockTargetActor : AbilityTargetActor
    {
        public bool WasStarted;

        public override void StartTargeting(Ability ability)
        {
            base.StartTargeting(ability);
            WasStarted = true;
        }

        protected override TargetDataHandle GetTargetData()
        {
            var handle = new TargetDataHandle();
            handle.Add(new TargetDataLocation { Position = Vector3.one });
            return handle;
        }
    }

    public class MockAbilityForTargeting : Ability
    {
        public MockAbilityForTargeting(AbilityDefinition definition, IAbilitySystem owner) : base(definition, owner)
        {
        }

        public bool Confirmed;
        public bool Cancelled;
        public TargetDataHandle ReceivedData;

        public WaitTargetDataTask TargetingTask;

        protected override void ActivateAbility(AbilityData data)
        {
            TargetingTask.OnTargetDataReceived += OnConfirmed;
            TargetingTask.OnCancelled += OnCancelledEvent;
            TargetingTask.ReadyForActivation();
        }

        public override void EndAbility()
        {
        }

        private void OnConfirmed(TargetDataHandle data)
        {
            Confirmed = true;
            ReceivedData = data;
        }

        private void OnCancelledEvent()
        {
            Cancelled = true;
        }
    }

    public class AbilityTargetActorTests : AbilitySystemTestBase
    {
        private GameObject _prefabGo;
        private MockTargetActor _prefabActor;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            SourceMock.Setup(s => s.IsLocalClient()).Returns(true);
            
            _prefabGo = new GameObject("MockTargetActorPrefab");
            _prefabActor = _prefabGo.AddComponent<MockTargetActor>();
            _prefabGo.SetActive(false); // Make it a "prefab"
        }

        [TearDown]
        public override void TearDown()
        {
            if (_prefabGo != null)
            {
                Object.DestroyImmediate(_prefabGo);
            }
            base.TearDown();
        }

        [Test]
        public void WaitTargetDataTask_SpawnsActorAndWaitsForConfirmation()
        {
            var definition = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            var ability = new MockAbilityForTargeting(definition, Source);

            var task = WaitTargetDataTask.CreateWaitTargetData(ability, _prefabActor);
            ability.TargetingTask = task;

            // Start ability, which will activate the task
            ability.TryActivateAbility(new AbilityData());

            // The task should have spawned a clone of the actor
            var spawnedActor = Object.FindAnyObjectByType<MockTargetActor>();
            Assert.IsNotNull(spawnedActor, "The target actor should have been spawned.");
            Assert.AreNotEqual(_prefabActor, spawnedActor, "A clone should have been spawned, not the prefab itself.");
            Assert.IsTrue(spawnedActor.WasStarted, "StartTargeting should be called on the spawned actor.");

            // Confirm targeting on the spawned actor
            spawnedActor.ConfirmTargeting();

            Assert.IsTrue(ability.Confirmed, "Ability should have received the confirmed target data.");
            Assert.IsFalse(ability.Cancelled, "Ability should not be cancelled.");
            Assert.AreEqual(1, ability.ReceivedData.Data.Count);
            
            var locData = (TargetDataLocation)ability.ReceivedData.Data[0];
            Assert.AreEqual(Vector3.one, locData.Position);

            // DestroyImmediate is synchronous in edit mode - the object should be null now
            Assert.IsTrue((UnityEngine.Object)spawnedActor == null, "Target actor should be destroyed after confirmation.");
        }

        [Test]
        public void WaitTargetDataTask_HandlesCancellation()
        {
            var definition = ScriptableObject.CreateInstance<TestAbilityDefinition>();
            var ability = new MockAbilityForTargeting(definition, Source);

            var task = WaitTargetDataTask.CreateWaitTargetData(ability, _prefabActor);
            ability.TargetingTask = task;

            ability.TryActivateAbility(new AbilityData());

            var spawnedActor = Object.FindAnyObjectByType<MockTargetActor>();
            Assert.IsNotNull(spawnedActor);

            // Cancel targeting
            spawnedActor.CancelTargeting();

            Assert.IsFalse(ability.Confirmed, "Ability should not receive confirmed data.");
            Assert.IsTrue(ability.Cancelled, "Ability should have been cancelled.");
            Assert.IsTrue((UnityEngine.Object)spawnedActor == null, "Target actor should be destroyed after cancellation.");
        }
    }
}
