using System.Linq;
using AbilityGraph.Runtime.Nodes.Abilities;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilityGraph.Runtime
{
    public class GraphAbility : Ability
    {
        private readonly AbilityGraphDefinition _graph;
        private readonly GraphContext _context;
        private readonly GraphRunner _activateRunner;
        private readonly GraphRunner _endRunner;

        public GraphAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
            // Deep-copy the asset so this instance's node state is isolated from other actors.
            _graph = ScriptableObject.Instantiate(ability as AbilityGraphDefinition);

            _context = new GraphContext(this, owner);

            // Initialize all AbilityNodes with the shared context.
            foreach (var node in _graph.nodes)
            {
                if (node is AbilityNode abilityNode)
                    abilityNode.Initialise(_context);
            }

            // Build activate runner from the ActivateAbilityNode.
            var activateNode = _graph.nodes.OfType<ActivateAbilityNode>().FirstOrDefault();
            if (activateNode != null)
                _activateRunner = new GraphRunner(activateNode, _context);

            // Build a dedicated end runner from the EndAbilityNode so that
            // end-ability graphs are first-class and support waitable nodes.
            var endNode = _graph.nodes.OfType<EndAbilityNode>().FirstOrDefault();
            if (endNode != null)
                _endRunner = new GraphRunner(endNode, _context);
        }

        protected override void ActivateAbility(AbilityData data)
        {
            if (_activateRunner == null) return;
            _context.SetActivationData(data);
            _activateRunner.Run();
        }

        public override void EndAbility()
        {
            // Cancel any in-flight waitable nodes on the activate runner.
            _activateRunner?.Cancel();
            _endRunner?.Run();
        }
    }
}