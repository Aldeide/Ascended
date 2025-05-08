using System.Linq;
using AbilityGraph.Runtime.Nodes;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime
{
    public class TestAbilityGraph : Ability
    {
        private readonly AbilityGraphDefinition _graphDefinition;
        private readonly ActivateAbilityNode _activateNode;
        private readonly EndAbilityNode _endNode;
        public TestAbilityGraph(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
            Debug.Log("Creating Graph");
            _graphDefinition = ability as AbilityGraphDefinition;
            if (_graphDefinition != null)
            {
                Debug.Log("Creating Graph not null");
                _activateNode = _graphDefinition.graph.nodes.FirstOrDefault(n => n is ActivateAbilityNode) as ActivateAbilityNode;
                _endNode = _graphDefinition.graph.nodes.FirstOrDefault(n => n is EndAbilityNode) as EndAbilityNode;
            }
        }

        protected override void ActivateAbility(AbilityData data)
        {
            Debug.Log("Activate graph!");
            if (_activateNode == null) return;
            Debug.Log("Activate graph 2!");
            var exec = _activateNode.GetExecutedNodes();
            foreach (var n in exec)
            {
                n.OnProcess();
            }
        }

        public override void EndAbility()
        {
            if (_endNode == null) return;
            var exec = _endNode.GetExecutedNodes();
            foreach (var n in exec)
            {
                n.OnProcess();
            }
        }
    }
}