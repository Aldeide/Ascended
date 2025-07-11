using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Effects;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/CommitCostAndCooldown")]
    public class CommitCostAndCooldown : LinearExecutableNode
    {
        public override string name => "Ability: Commit Cost and Cooldown";
        protected override void Process()
        {
            Ability.CommitCostAndCooldown();
        }
    }
}