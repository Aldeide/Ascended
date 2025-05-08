using System.Collections.Generic;
using System.Linq;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes
{
    [System.Serializable, NodeMenuItem("Abilities/ActivateAbility")]
    public class ActivateAbilityNode : AbilityStartNode
    {
        public override string name => "Activate Ability";
    }
}