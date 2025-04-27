namespace AbilitySystem.Runtime.Cues
{
    public abstract class DurationalCue : Cue
    {
        public abstract void Add();
        public abstract void Remove();

        protected DurationalCue(CueDefinition definition) : base(definition)
        {
        }
    }
}