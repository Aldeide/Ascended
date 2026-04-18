using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Cues;
using GameplayTags.Runtime;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Cues
{
    [Serializable, NodeMenuItem("Cues/Play Cue At Location")]
    public class PlayCueAtLocationNode : LinearExecutableNode
    {
        [Input(name = "Cue Tag")]
        public Tag CueTag;

        [Input(name = "Location")]
        public Vector3 Location;

        protected override void Process()
        {
            if (Owner == null || CueTag == null) return;

            var data = new CueData
            {
                VectorData = new[] { Location, Vector3.one, Vector3.one }
            };

            Owner.PlayCue(CueTag.Name, data, Owner.IsLocalClient());
        }
    }
}
