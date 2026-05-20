using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.AbilityTasks;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/Wait Net Sync")]
    public class WaitNetSyncNode : WaitableNode
    {
        [SerializeField]
        public AbilityNetSyncType SyncType = AbilityNetSyncType.BothWait;

        private WaitNetSyncTask _task;

        protected override void Process()
        {
            if (Ability == null)
            {
                ProcessFinished();
                return;
            }

            _task = WaitNetSyncTask.CreateWaitNetSync(Ability, SyncType);
            _task.OnSyncFinished += OnSyncFinished;
            _task.ReadyForActivation();
        }

        private void OnSyncFinished()
        {
            if (_task != null)
            {
                _task.OnSyncFinished -= OnSyncFinished;
            }
            ProcessFinished();
        }
    }
}
