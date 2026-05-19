using System;
using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.Networking
{
    public readonly struct ScopedAbilityRPCBatcher : IDisposable
    {
        private readonly IReplicationManager _replicationManager;

        public ScopedAbilityRPCBatcher(IReplicationManager replicationManager)
        {
            _replicationManager = replicationManager;
            _replicationManager.BeginBatch();
        }

        public void Dispose()
        {
            _replicationManager.EndBatch();
        }
    }
}
