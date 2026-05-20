using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Base
{
    [Serializable]
    public abstract class AbilityNode : BaseNode
    {
        protected GraphContext Context { get; private set; }

        // Convenience accessors kept for backwards compatibility with existing nodes.
        protected Ability Ability => Context?.Ability;
        protected IAbilitySystem Owner => Context?.Owner;

        /// <summary>
        /// Called once per ability instance construction. Override to cache ports or
        /// perform any one-time setup that does not belong in Process().
        /// </summary>
        public virtual void Initialise(GraphContext context)
        {
            Context = context;
        }
    }
}