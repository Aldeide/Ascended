using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using NewGraph;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AbilityGraph
{
    public class AbilityGraph : Ability
    {
        public AbilityGraph(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        protected override void ActivateAbility(AbilityData data)
        {
            var activateNode =
                (Definition as AbilityGraphDefinition).abilityGraph.Nodes.Where(n =>
                    n.GetName() == "ActivateAbilityNode").First();
            (activateNode.nodeData as AbilityNode).Execute();

            (activateNode.nodeData as AbilityNode).nextNode.Execute();
        }

        public override void EndAbility()
        {
            throw new NotImplementedException();
        }
    }
}