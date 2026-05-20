using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Abilities.Targeting;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/Wait Target Data")]
    public class WaitTargetDataNode : WaitableNode
    {
        [SerializeField]
        public AbilityTargetActor TargetActorPrefab;

        [Output(name = "Target Data")]
        public TargetDataHandle TargetData;

        private WaitTargetDataTask _task;

        protected override void Process()
        {
            if (Ability == null)
            {
                ProcessFinished();
                return;
            }

            _task = WaitTargetDataTask.CreateWaitTargetData(Ability, TargetActorPrefab);
            _task.OnTargetDataReceived += OnTargetDataReceived;
            _task.OnCancelled += OnTargetDataCancelled;
            _task.ReadyForActivation();
        }

        private void OnTargetDataReceived(TargetDataHandle data)
        {
            TargetData = data;
            Context.TargetData = data;
            Cleanup();
            ProcessFinished();
        }

        private void OnTargetDataCancelled()
        {
            Cleanup();
            ProcessFinished();
        }

        private void Cleanup()
        {
            if (_task != null)
            {
                _task.OnTargetDataReceived -= OnTargetDataReceived;
                _task.OnCancelled -= OnTargetDataCancelled;
            }
        }
    }
}
