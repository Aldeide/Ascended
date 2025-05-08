using System.Collections.Generic;
using System.Linq;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes
{
    [System.Serializable, NodeMenuItem("Abilities/EndAbility")]
    public class EndAbilityNode : BaseNode, IExecutableNode
    {
        [Output(name = "Executes")]
        public ExecutableLink Executes;
        public override string name => "End Ability";
        public IEnumerable<ExecutableNode> GetExecutedNodes()
        {
            return GetOutputNodes().OfType<ExecutableNode>();
        }
    }
}