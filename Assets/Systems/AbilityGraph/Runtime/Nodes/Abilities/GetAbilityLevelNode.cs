using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/Get Ability Level")]
    public class GetAbilityLevelNode : AbilityNode
    {
        [Output(name = "Level")]
        public int Level;

        protected override void Process()
        {
            if (Ability == null)
            {
                Level = 1;
                return;
            }
            Level = Ability.Level;
        }
    }
}
