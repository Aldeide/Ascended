using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Networking;

namespace AbilitySystem.Runtime.AbilityTasks
{
    public class WaitNetSyncTask : AbilityTask
    {
        public event Action OnSyncFinished;

        private AbilityNetSyncType _syncType;
        private PredictionKey _predictedKey;
        private bool _hasSynced;

        public static WaitNetSyncTask CreateWaitNetSync(Ability owningAbility, AbilityNetSyncType syncType)
        {
            var task = new WaitNetSyncTask();
            task.Initialize(owningAbility);
            task._syncType = syncType;
            return task;
        }

        protected override void Activate()
        {
            var rep = OwnerSystem.ReplicationManager;
            var isClient = OwnerSystem.IsLocalClient() && !OwnerSystem.IsServer();

            if (isClient)
            {
                // Generate a dependent prediction key for the new sync point
                _predictedKey = PredictionKey.CreateDependentPredictionKey(OwningAbility.PredictionKey);
                
                // Send sync key to server
                rep.SendSyncKey(OwningAbility.Definition.UniqueName, _predictedKey);

                if (_syncType == AbilityNetSyncType.OnlyServerWait)
                {
                    // Client does not wait - immediately apply new key and fire callback
                    OwningAbility.SetPredictionKey(_predictedKey);
                    CompleteTask();
                }
                else // BothWait
                {
                    rep.OnClientSyncKeyConfirmed += OnSyncKeyConfirmed;
                }
            }
            else // Server
            {
                // Server checks if client's sync key has already been queued
                if (OwningAbility.TryConsumeSyncKey(out var consumedKey))
                {
                    ProcessServerSync(consumedKey);
                }
                else
                {
                    OwningAbility.OnSyncKeyReceived += OnServerSyncKeyReceived;
                }
            }
        }

        private void OnSyncKeyConfirmed(string abilityName, PredictionKey key)
        {
            if (abilityName != OwningAbility.Definition.UniqueName || key.currentKey != _predictedKey.currentKey) return;
            
            // Apply new key and complete
            OwningAbility.SetPredictionKey(key);
            CompleteTask();
        }

        private void OnServerSyncKeyReceived(PredictionKey key)
        {
            if (OwningAbility.TryConsumeSyncKey(out var consumedKey))
            {
                // Unsubscribe and process
                OwningAbility.OnSyncKeyReceived -= OnServerSyncKeyReceived;
                ProcessServerSync(consumedKey);
            }
        }

        private void ProcessServerSync(PredictionKey key)
        {
            OwningAbility.SetPredictionKey(key);

            if (_syncType == AbilityNetSyncType.BothWait)
            {
                // Inform client that server has caught up to the sync point
                OwnerSystem.ReplicationManager.ConfirmSyncKey(OwningAbility.Definition.UniqueName, key);
            }

            CompleteTask();
        }

        private void CompleteTask()
        {
            if (_hasSynced) return;
            _hasSynced = true;
            OnSyncFinished?.Invoke();
            EndTask();
        }

        protected override void OnDestroy()
        {
            if (OwnerSystem != null && OwnerSystem.ReplicationManager != null)
            {
                OwnerSystem.ReplicationManager.OnClientSyncKeyConfirmed -= OnSyncKeyConfirmed;
            }
            if (OwningAbility != null)
            {
                OwningAbility.OnSyncKeyReceived -= OnServerSyncKeyReceived;
            }
            OnSyncFinished = null;
        }
    }

    public enum AbilityNetSyncType
    {
        BothWait,
        OnlyServerWait
    }
}
