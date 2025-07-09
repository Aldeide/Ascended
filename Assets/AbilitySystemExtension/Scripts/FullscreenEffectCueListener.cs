using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using GameplayTags.Runtime;
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
    public class FullscreenEffectCueListener : MonoBehaviour, ICueListener
    {
        public TagQuery TagQuery { get; set; }

        private CueManagerComponent _cueManager;

        private void Start()
        {
            _cueManager = GetComponent<CueManagerComponent>();
            _cueManager.OnCueAdded += OnCueAdded;
        }

        private void OnCueAdded(string cueTag, CueDefinition definition)
        {
            if (!TagQuery.MatchesTag(definition.cueTag)) return;
            // TODO: Play fullscreen effect
        }
    }
}