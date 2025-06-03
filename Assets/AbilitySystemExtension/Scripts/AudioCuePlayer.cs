using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    public class AudioCuePlayer : MonoBehaviour, ICueListener
    {
        [ShowInInspector]
        public GameplayTag[] TagFilter { get; set; }
        
        [ShowInInspector]
        public GameplayTagQuery TagQuery { get; set; }

        [ShowInInspector] private AudioSource _audioSource;
        private CueManagerComponent _cueManager;

        public void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _cueManager = GetComponentInParent<CueManagerComponent>();
            _cueManager.OnCueAdded += OnCueAdded;
        }

        private void OnCueAdded(string cueTag, CueDefinition definition)
        {
            if (!cueTag.StartsWith("Cue.Audio")) return;
            _audioSource.clip = (definition as CueAudioDefinition)?.AudioClip;
            Debug.Log("Playing audio");
            _audioSource.Play();
        }
    }
}