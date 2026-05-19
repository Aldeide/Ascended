using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    // Component that handles the execution of cues to run cosmetics (vfx, audio, animation, ...).
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class CueManagerComponent : NetworkBehaviour
    {
        private IAbilitySystem _abilitySystem;
        private CueDefinitionLibrary _cueLibrary;
        
        public Action<string, CueDefinition> OnCueAdded; 
        
        public void Start()
        {
            _abilitySystem = GetComponent<AbilitySystemComponent>().AbilitySystem;
            TryFindLibrary();
        }

        private void TryFindLibrary()
        {
            if (_cueLibrary) return;
            var dataManager = GameObject.Find("DataManager");
            if (dataManager)
            {
                _cueLibrary = dataManager.GetComponent<CueDefinitionLibrary>();
            }
        }

        public void PlayCue(string cueTag)
        {
            TryFindLibrary();
            if (!_cueLibrary) return;

            var cue = _cueLibrary.GetCueByTag(cueTag);
            OnCueAdded?.Invoke(cueTag, cue);
        }

        public void PlayCue(string cueTag, CueData data)
        {
            TryFindLibrary();
            if (!_cueLibrary) return;

            var cue = _cueLibrary.GetCueByTag(cueTag);
            if (!cue) return;
            
            OnCueAdded?.Invoke(cueTag, cue);
            
            // Also notify the core CueManager so ICueListeners can respond.
            _abilitySystem.CueManager.OnCueReceived(new GameplayTags.Runtime.Tag(cueTag), CueAction.Execute, data);
        }
    }
}