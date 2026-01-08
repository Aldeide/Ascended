using System;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Base
{
    [Serializable]
    public abstract class LinearExecutableNode : ExecutableNode, IExecutableNode
    {
        [Output(name = "Executes")]
        public ExecutableLink Executes;

        public override IEnumerable<ExecutableNode>	GetExecutedNodes()
        {
            // Return all the nodes connected to the executes port
            return outputPorts.FirstOrDefault(n => n.fieldName == nameof(Executes))
                ?.GetEdges().Select(e => e.inputNode as ExecutableNode);
        }
    }
}