using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Cues;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Cues
{
    [System.Serializable, NodeMenuItem("Cues/SendCue")]
    public class SendCue : LinearExecutableNode
    {
        public override string name => "Send Cue";
        
        public CueDefinition CueDefinition;
        
        protected override void Process()
        {
            Owner.PlayCue(CueDefinition);
        }
    }
}