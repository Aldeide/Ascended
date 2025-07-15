using GameplayTags.Runtime;

namespace AbilitySystem.Runtime.Cues
{
    public abstract class DurationalCue : ICue
    {
        public Tag CueTag { get; set; }
        
        public abstract void Start();

        public abstract void Stop();

        public abstract void Catchup();
    }
}