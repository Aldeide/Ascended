using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    public class AudioCuePlayer : MonoBehaviour, ICueListener
    {
        public GameplayTag[] TagFilter { get; set; }

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
            _audioSource.clip = (definition as CueAudioDefinition)?.audioClip;
            Debug.Log("Playing audio");
            _audioSource.Play();
        }
    }
}