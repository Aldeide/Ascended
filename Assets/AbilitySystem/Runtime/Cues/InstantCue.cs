using System;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    public abstract class InstantCue : Cue
    {
        public abstract void Execute();

        protected InstantCue(InstantCueDefinition definition) : base(definition)
        {
        }
    }
}