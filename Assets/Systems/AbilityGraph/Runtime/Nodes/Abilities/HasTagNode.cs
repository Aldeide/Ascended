using System;
using AbilityGraph.Runtime.Nodes.Base;
using GameplayTags.Runtime;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/Has Tag")]
    public class HasTagNode : AbilityNode
    {
        [Input(name = "Tag")]
        public Tag Tag;

        [Output(name = "Has Tag")]
        public bool HasTag;

        protected override void Process()
        {
            if (Owner == null || Tag == null)
            {
                HasTag = false;
                return;
            }
            HasTag = Owner.TagManager.HasTag(Tag);
        }
    }
}
