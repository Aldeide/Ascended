using System;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Base
{
    [Serializable]
    public class AbilityStartNode : AbilityNode, IExecutableNode
    {
        [Output(name = "Executes")]
        public ExecutableLink Executes;
        public override string name => "Start Node";
        public IEnumerable<ExecutableNode> GetExecutedNodes()
        {
            return GetOutputNodes().OfType<ExecutableNode>();
        }
    }
}
