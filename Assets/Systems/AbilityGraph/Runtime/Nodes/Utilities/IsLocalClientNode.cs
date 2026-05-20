using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Utilities
{
    [Serializable, NodeMenuItem("Utilities/Is Local Client")]
    public class IsLocalClientNode : AbilityNode
    {
        [Output(name = "Is Local")]
        public bool IsLocal;

        protected override void Process()
        {
            IsLocal = Owner != null && Owner.IsLocalClient();
        }
    }
}
