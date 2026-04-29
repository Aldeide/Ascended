using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.Targeting;

namespace AbilitySystem.Runtime.AbilityTasks
{
    public class WaitTargetDataTask : AbilityTask
    {
        public event Action<TargetDataHandle> OnTargetDataReceived;
        public event Action OnCancelled;

        public static WaitTargetDataTask CreateWaitTargetData(Ability owningAbility)
        {
            var task = new WaitTargetDataTask();
            task.Initialize(owningAbility);
            return task;
        }

        protected override void Activate()
        {
            // On the server, we wait for an RPC payload.
            // On the local client, we wait for local input to call ConfirmTargetData.
        }

        public void ConfirmTargetData(TargetDataHandle data)
        {
            if (!IsActive) return;
            
            OnTargetDataReceived?.Invoke(data);
            EndTask();
        }

        public void CancelTargetData()
        {
            if (!IsActive) return;
            
            OnCancelled?.Invoke();
            EndTask();
        }

        protected override void OnDestroy()
        {
            OnTargetDataReceived = null;
            OnCancelled = null;
        }
    }
}
