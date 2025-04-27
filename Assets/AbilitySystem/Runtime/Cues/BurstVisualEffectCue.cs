using UnityEngine.VFX;

namespace AbilitySystem.Runtime.Cues
{
    public class BurstVisualEffectCue : InstantCue
    {
        public VisualEffect VisualEffect;
        
        public override void Execute()
        {
            throw new System.NotImplementedException();
        }

        public BurstVisualEffectCue(InstantCueDefinition definition) : base(definition)
        {
        }
    }
}