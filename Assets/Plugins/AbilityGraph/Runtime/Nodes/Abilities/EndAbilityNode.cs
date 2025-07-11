using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [System.Serializable, NodeMenuItem("Abilities/EndAbility")]
    public class EndAbilityNode : AbilityStartNode
    {
        public override string name => "End Ability";
    }
}