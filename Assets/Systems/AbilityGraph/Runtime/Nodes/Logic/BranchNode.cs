using System.Collections.Generic;
using System.Linq;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Logic
{
    public class BranchNode : ExecutableNode, IExecutableNode
    {
        [Input(name = "Condition")]
        public bool Condition;
        
        [Output(name = "ExecutesIfTrue")]
        public ExecutableLink ExecutesIfTrue;

        [Output(name = "ExecutesIfFalse")] public ExecutableLink ExecutesIfFalse;
        
        public override IEnumerable<ExecutableNode>	GetExecutedNodes()
        {
            if (Condition)
            {
                return outputPorts.FirstOrDefault(n => n.fieldName == nameof(ExecutesIfTrue))
                    ?.GetEdges().Select(e => e.inputNode as ExecutableNode);
            }
            return outputPorts.FirstOrDefault(n => n.fieldName == nameof(ExecutesIfFalse))
                ?.GetEdges().Select(e => e.inputNode as ExecutableNode);
        }
    }
}