using System.Collections.Generic;
using GameplayTags.Runtime;

namespace AbilitySystem.Runtime.Cues
{
    public interface ICueListener
    {
        public TagQuery TagQuery { get; set; }
        public List<DurationalCue> ActiveCues { get; set; }
        public CueManager CueManager { get; set; }
        public void OnExecuteCue(CueDefinition definition, CueData cueData);
        public void OnPlayCue(CueDefinition definition, CueData cueData);
        public void OnStopCue(CueDefinition definition, CueData cueData);
    }
}