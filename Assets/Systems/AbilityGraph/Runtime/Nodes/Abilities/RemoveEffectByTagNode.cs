using System;
using System.Collections.Generic;
using System.Linq;
using AbilityGraph.Runtime.Nodes.Base;
using GameplayTags.Runtime;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/Remove Effect By Tag")]
    public class RemoveEffectByTagNode : LinearExecutableNode
    {
        [Input(name = "Tag")]
        public Tag Tag;

        protected override void Process()
        {
            if (Owner == null || Tag == null) return;

            var effectsToRemove = Owner.EffectManager.Effects.Where(e => 
                (e.Definition.AssetTags != null && e.Definition.AssetTags.Contains(Tag)) ||
                (e.Definition.GrantedTags != null && e.Definition.GrantedTags.Contains(Tag))
            ).ToList();

            foreach (var effect in effectsToRemove)
            {
                Owner.EffectManager.RemoveEffect(effect);
            }
        }
    }
}
