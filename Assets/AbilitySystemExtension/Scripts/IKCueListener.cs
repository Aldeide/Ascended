using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    public class IKCueListener: MonoBehaviour, ICueListener
    {
        public GameplayTag[] TagFilter { get; set; }
        
        [ShowInInspector]
        public GameplayTagQuery TagQuery { get; set; }
        
        private CueManagerComponent _cueManager;
        
        private void Start()
        {
            _cueManager = GetComponent<CueManagerComponent>();
            _cueManager.OnCueAdded += OnCueAdded;
        }

        private void OnCueAdded(string cueTag, CueDefinition definition)
        {
            if (cueTag == "Cue.IK.Arm.Right.Disable")
            {
                
            }
        }
    }
}