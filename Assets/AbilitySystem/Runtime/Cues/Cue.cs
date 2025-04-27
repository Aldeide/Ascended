using System;
using AbilitySystem.Runtime.Tags;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    public abstract class Cue
    {
        public GameplayTag CueTag;

        public Cue(CueDefinition definition)
        {
            
        }
    }
}