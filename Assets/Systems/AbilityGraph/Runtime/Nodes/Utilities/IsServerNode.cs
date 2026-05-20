using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Utilities
{
    [Serializable, NodeMenuItem("Utilities/Is Server")]
    public class IsServerNode : AbilityNode
    {
        [Output(name = "Is Server")]
        public bool IsServer;

        protected override void Process()
        {
            IsServer = Owner != null && Owner.IsServer();
        }
    }
}
