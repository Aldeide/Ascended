using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using GraphProcessor;
using PlasticGui.WorkspaceWindow;

namespace AbilityGraph.Runtime.Nodes.Base
{
    public abstract class AbilityNode : BaseNode
    {
        protected Ability Ability;
        protected IAbilitySystem Owner;

        public virtual void Initialise(Ability ability)
        {
            Ability = ability;
            Owner = Ability.Owner;
        }
    }
}