using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.Targeting;
using AbilitySystem.Runtime.Core;

namespace AbilityGraph.Runtime
{
    /// <summary>
    /// Carries all runtime context for a single ability graph execution.
    /// Passed to GraphRunner and made available to all AbilityNodes via Initialise().
    /// Separates per-activation state from the shared graph asset.
    /// </summary>
    public class GraphContext
    {
        /// <summary>The ability instance that owns this graph execution.</summary>
        public Ability Ability { get; }

        /// <summary>The ability system that owns the ability.</summary>
        public IAbilitySystem Owner { get; }

        /// <summary>The data passed at activation (e.g., initial target, payload).</summary>
        public AbilityData ActivationData { get; private set; }

        /// <summary>
        /// Target data collected during graph execution (e.g., from WaitTargetDataNode).
        /// Updated by targeting nodes so later nodes can consume it.
        /// </summary>
        public TargetDataHandle TargetData { get; set; }

        public GraphContext(Ability ability, IAbilitySystem owner)
        {
            Ability = ability;
            Owner = owner;
        }

        /// <summary>Called by GraphAbility at the start of ActivateAbility.</summary>
        public void SetActivationData(AbilityData data)
        {
            ActivationData = data;
            TargetData = default;
        }
    }
}
