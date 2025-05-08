using System;
using System.Collections.Generic;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Effects;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/ApplyEffectToOwner")]
    public class ApplyEffectToOwnerNode : LinearExecutableNode
    {
        [SerializeField]
        public EffectDefinition EffectDefinition;
        protected override void Process()
        {
            var effect = EffectDefinition.ToEffect(Owner, Owner);
            effect.Activate();
            Owner.EffectManager.AddEffect(effect);
        }
    }
}