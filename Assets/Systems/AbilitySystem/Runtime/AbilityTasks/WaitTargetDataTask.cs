using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.Targeting;

namespace AbilitySystem.Runtime.AbilityTasks
{
    public class WaitTargetDataTask : AbilityTask
    {
        public event Action<TargetDataHandle> OnTargetDataReceived;
        public event Action OnCancelled;

        private AbilityTargetActor _targetActorPrefab;
        private AbilityTargetActor _spawnedActor;

        public static WaitTargetDataTask CreateWaitTargetData(Ability owningAbility, AbilityTargetActor targetActorPrefab = null)
        {
            var task = new WaitTargetDataTask();
            task._targetActorPrefab = targetActorPrefab;
            task.Initialize(owningAbility);
            return task;
        }

        protected override void Activate()
        {
            // Spawn the target actor only on the local client
            if (OwningAbility.Owner.IsLocalClient() && _targetActorPrefab != null)
            {
                _spawnedActor = UnityEngine.Object.Instantiate(_targetActorPrefab);
                _spawnedActor.gameObject.SetActive(true); // Activate - prefab may be inactive
                _spawnedActor.OnTargetDataReady += ConfirmTargetData;
                _spawnedActor.OnTargetDataCancelled += CancelTargetData;
                _spawnedActor.StartTargeting(OwningAbility);
            }
        }

        public void ConfirmTargetData(TargetDataHandle data)
        {
            if (!IsActive) return;

            // Null out the reference BEFORE EndTask() triggers OnDestroy.
            // The actor will destroy itself after firing the event.
            _spawnedActor = null;
            OnTargetDataReceived?.Invoke(data);
            EndTask();
        }

        public void CancelTargetData()
        {
            if (!IsActive) return;

            // Null out the reference BEFORE EndTask() triggers OnDestroy.
            // The actor will destroy itself after firing the event.
            _spawnedActor = null;
            OnCancelled?.Invoke();
            EndTask();
        }

        protected override void OnDestroy()
        {
            // _spawnedActor is only non-null here if the ability was interrupted
            // externally (e.g. ability cancelled by another system) rather than
            // through the normal Confirm/Cancel flow.
            if (_spawnedActor != null)
            {
                _spawnedActor.OnTargetDataReady -= ConfirmTargetData;
                _spawnedActor.OnTargetDataCancelled -= CancelTargetData;

                // CancelTargeting will fire OnTargetDataCancelled and destroy the actor.
                if (_spawnedActor.IsTargeting)
                {
                    _spawnedActor.CancelTargeting();
                }
                else if ((UnityEngine.Object)_spawnedActor != null)
                {
                    // Orphaned inactive actor - destroy it directly.
                    if (UnityEngine.Application.isPlaying)
                        UnityEngine.Object.Destroy(_spawnedActor.gameObject);
                    else
                        UnityEngine.Object.DestroyImmediate(_spawnedActor.gameObject);
                }
            }

            OnTargetDataReceived = null;
            OnCancelled = null;
        }
    }
}
