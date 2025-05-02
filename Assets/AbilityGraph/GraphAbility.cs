using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using NewGraph;

namespace AbilityGraph
{
    public class GraphAbility : Ability
    {
        private ScriptableGraphModel _graph;
        
        public GraphAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        protected override void ActivateAbility(AbilityData data)
        {
            
        }

        public override void EndAbility()
        {
            throw new System.NotImplementedException();
        }
    }
}