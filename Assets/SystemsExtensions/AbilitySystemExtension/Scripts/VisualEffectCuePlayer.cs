using AbilitySystem.Runtime.Cues;
using UnityEngine.VFX;

namespace AbilitySystemExtension.Scripts
{
    public class VisualEffectCuePlayer : CueListenerComponent
    {
        public VisualEffect VisualEffect;

        public override void Start()
        {
            base.Start();
            VisualEffect = GetComponent<VisualEffect>();
        }

        public override void OnExecuteCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
            VisualEffect.Play();
        }

        public override void OnPlayCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
            var asset = (definition as VisualEffectCueDefinition)?.VisualEffectAsset;
            VisualEffect.visualEffectAsset = asset;
            VisualEffect.Play();
        }

        public override void OnStopCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
            VisualEffect.Stop();
        }
    }
}