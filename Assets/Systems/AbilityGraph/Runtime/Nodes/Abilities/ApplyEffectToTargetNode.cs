using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/ApplyEffectToTarget")]
    public class ApplyEffectToTargetNode : LinearExecutableNode
    {
        [Input(name = "Target")]
        public GameObject Target;

        [Input(name = "Level")]
        public int Level = 1;

        [SerializeField]
        public EffectDefinition EffectDefinition;

        [SerializeField, Tooltip("If true, this effect will only be applied on the server. Essential for damage to avoid client-side double-dipping.")]
        public bool ServerOnly = true;

        protected override void Process()
        {
            if (Owner == null || Target == null || EffectDefinition == null)
            {
                return;
            }

            if (ServerOnly && !Owner.IsServer())
            {
                return;
            }

            var targetAbilitySystem = Target.GetComponent<IAbilitySystem>();
            if (targetAbilitySystem != null)
            {
                // Create the effect using the Owner's context (as the source)
                var effect = Owner.MakeOutgoingEffect(EffectDefinition, Level);
                
                // If we are applying it on the client as a predicted effect (ServerOnly = false),
                // we should pass down the PredictionKey so it gets registered correctly.
                if (Ability != null)
                {
                    effect.PredictionKey = Ability.PredictionKey;
                }

                // Apply to target
                targetAbilitySystem.ApplyEffectToSelf(effect);
            }
        }
    }
}
