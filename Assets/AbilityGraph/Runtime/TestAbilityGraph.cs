using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using AbilityGraph.Runtime.Nodes;
using AbilityGraph.Runtime.Nodes.Abilities;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime
{
    public class TestAbilityGraph : Ability
    {
        private readonly AbilityGraphDefinition _graphDefinition;
        private readonly AbilityGraph _graph;
        private readonly ActivateAbilityNode _activateNode;
        private readonly EndAbilityNode _endNode;
        private readonly GraphRunner _activateRunner;
        public TestAbilityGraph(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
            _graphDefinition = (ability as AbilityGraphDefinition);
            _graph = ScriptableObject.Instantiate(_graphDefinition.graph);
            if (_graphDefinition != null)
            {
                _activateNode = _graph.nodes.FirstOrDefault(n => n is ActivateAbilityNode) as ActivateAbilityNode;
                _endNode = _graph.nodes.FirstOrDefault(n => n is EndAbilityNode) as EndAbilityNode;
                _graph.nodes.FindAll(n=>n is AbilityNode).ForEach(n=> (n as AbilityNode)?.Initialise(this));
                _activateRunner = new GraphRunner(_activateNode, this);
            }
        }

        protected override void ActivateAbility(AbilityData data)
        {
            Debug.Log("Activate graph!");
            if (_activateNode == null) return;
            _activateRunner.Run();
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