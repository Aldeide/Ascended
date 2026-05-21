using System;
using AbilityGraph.Runtime.Nodes.Base;
using GameplayTags.Runtime;
using GraphProcessor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Primitives
{
    [Serializable]
    [NodeMenuItem("Primitives/Tag")]
    public class TagNode : AbilityNode {
        [Output("Out")] public Tag Output;

        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag Tag;

        public override string name => "Tag";

        protected override void Process() {
            Output = Tag;
        }
    }
}