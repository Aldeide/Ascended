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
            _cueLibrary = GameObject.Find("DataManager").GetComponent<CueDefinitionLibrary>();
        }

        public void PlayCue(string cueTag)
        {
            CueDefinition cue = _cueLibrary.GetCueByTag(cueTag);
            OnCueAdded?.Invoke(cueTag, cue);
        }

        public void PlayCue(string cueTag, CueData data)
        {
            if (!_abilitySystem.IsServer()) return;
            CueDefinition cue = _cueLibrary.GetCueByTag(cueTag);
        }
    }
}