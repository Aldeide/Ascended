using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes
{
    [System.Serializable, NodeMenuItem("Abilities/ActivateAbility")]
    public class ActivateAbilityNode : BaseNode, IExecutableNode
    {
        [Output(name = "Executes")]
        public ExecutableLink Executes;
        public override string name => "Activate Ability";
        public IEnumerable<ExecutableNode> GetExecutedNodes()
        {
            return GetOutputNodes().OfType<ExecutableNode>();
        }
    }
}