using System;
using AbilitySystem.Runtime.Abilities;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.Targeting
{
    /// <summary>
    /// Base class for target actors spawned by the WaitTargetDataTask.
    /// Handles visualization and async data collection for abilities.
    /// </summary>
    public abstract class AbilityTargetActor : MonoBehaviour
    {
        public bool IsTargeting { get; private set; }
        public Ability OwningAbility { get; private set; }

        public event Action<TargetDataHandle> OnTargetDataReady;
        public event Action OnTargetDataCancelled;

        public virtual void StartTargeting(Ability ability)
        {
            OwningAbility = ability;
            IsTargeting = true;
        }

        public virtual void ConfirmTargeting()
        {
            if (!IsTargeting) return;
            IsTargeting = false;

            var data = GetTargetData();
            OnTargetDataReady?.Invoke(data);
            DestroyActor();
        }

        public virtual void CancelTargeting()
        {
            if (!IsTargeting) return;
            IsTargeting = false;

            OnTargetDataCancelled?.Invoke();
            DestroyActor();
        }

        private void DestroyActor()
        {
            if (Application.isPlaying)
                Destroy(gameObject);
            else
                DestroyImmediate(gameObject);
        }

        protected abstract TargetDataHandle GetTargetData();
    }
}
