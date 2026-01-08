using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/CallEndAbility")]
    public class CallEndAbilityNode : LinearExecutableNode
    {
        public override string name => "Ability: Call End Ability";
        protected override void Process()
        {
            Ability.TryEndAbility();
        }
    }
}