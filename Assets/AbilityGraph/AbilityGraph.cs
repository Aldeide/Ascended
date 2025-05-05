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
        private ActivateAbilityNode _activateNode;
        
        public AbilityGraph(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
            _activateNode =
                (Definition as AbilityGraphDefinition)?.abilityGraph.Nodes.First(n =>
                    n.nodeType == typeof(ActivateAbilityNode)).nodeData as ActivateAbilityNode;
        }

        protected override void ActivateAbility(AbilityData data)
        {
            var activateNode =
                (Definition as AbilityGraphDefinition).abilityGraph.Nodes.Where(n =>
                    n.GetName() == "ActivateAbilityNode").First();
            (activateNode.nodeData as InstantAbilityNode).Execute();

            //(activateNode.nodeData as InstantAbilityNode).executeNext.Execute();
        }

        public override void EndAbility()
        {
            throw new NotImplementedException();
        }
    }
}