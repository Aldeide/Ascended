namespace AbilitySystem.Runtime.Cues
{
    public class AnimationCue : InstantCue
    {
        public string AnimationStateName;
        
        public override void Execute()
        {
            throw new System.NotImplementedException();
        }

        public AnimationCue(InstantCueDefinition definition) : base(definition)
        {
        }
    }
}