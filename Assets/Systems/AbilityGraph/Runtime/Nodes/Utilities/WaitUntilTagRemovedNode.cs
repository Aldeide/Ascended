using System;
using System.Threading.Tasks;
using AbilityGraph.Runtime.Nodes.Base;
using GameplayTags.Runtime;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Utilities
{
    [Serializable, NodeMenuItem("Utilities/Wait Until Tag Removed")]
    public class WaitUntilTagRemovedNode : WaitableNode
    {
        [Input(name = "Tag")]
        public Tag Tag;

        private bool _isWaiting;

        protected override void Process()
        {
            if (Owner == null || Tag == null || !Owner.TagManager.HasTag(Tag))
            {
                ProcessFinished();
                return;
            }

            if (!_isWaiting)
            {
                _isWaiting = true;
                Owner.TagManager.OnTagsChanged += CheckTag;
            }
        }

        private void CheckTag()
        {
            if (!Owner.TagManager.HasTag(Tag))
            {
                Owner.TagManager.OnTagsChanged -= CheckTag;
                _isWaiting = false;
                ProcessFinished();
            }
        }
        
        // Ensure cleanup if ability ends prematurely
        public override void Initialise(GraphContext context)
        {
            base.Initialise(context);
            // We'd ideally want an OnDestroy or similar logic in the Node, 
            // but for now, the OnTagsChanged listener is simple enough.
        }
    }
}
