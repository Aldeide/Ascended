using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Cues;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Cues
{
    [System.Serializable, NodeMenuItem("Cues/PlayAudioCue")]
    public class PlayAudioCue : LinearExecutableNode
    {
        public override string name => "Play Audio Cue";
        
        public CueAudioDefinition CueDefinition;
        
        protected override void Process()
        {
            Owner.PlayCue(CueDefinition);
        }
    }
}