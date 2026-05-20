using System;
using AbilityGraph.Runtime.Nodes.Base;
using GameplayTags.Runtime;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/Add Tag To Owner")]
    public class AddTagToOwnerNode : LinearExecutableNode
    {
        [Input(name = "Tag")]
        public Tag Tag;

        protected override void Process()
        {
            if (Owner != null && !string.IsNullOrEmpty(Tag.Name))
            {
                Owner.TagManager.AddTag(Tag);
            }
        }
    }
}
