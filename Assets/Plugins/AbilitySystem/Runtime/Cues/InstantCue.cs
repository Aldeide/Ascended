using GameplayTags.Runtime;

namespace AbilitySystem.Runtime.Cues
{
    public abstract class InstantCue : ICue
    {
        public Tag CueTag { get; set; }
        public abstract void Play();
    }
}