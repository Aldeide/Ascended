using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    /// This class implements the ICueListener interface and is used
    /// to listen for and handle cues based on specified gameplay tag queries within the context
    /// of the ability system. It requires the CueManagerComponent component to function.
    /// The FullscreenEffectCueListener class allows for the filtering of cues using
    /// a GameplayTagQuery to determine which cues the listener should respond to.
    /// This is to be attached to a gameobject that will play fullscreen effects (e.g., camera shake, damage taken
    /// visuals).
    [RequireComponent(typeof(CueManagerComponent))]
    public class FullscreenEffectCueListener : CueListenerComponent
    {
        public override void Start()
        {

        }

        public override void OnExecuteCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
        }

        public override void OnPlayCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
        }

        public override void OnStopCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
        }
    }
}