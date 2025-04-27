using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    public class AudioCuePlayer : MonoBehaviour
    {
        [ShowInInspector]
        private AudioSource _audioSource;
        private CueManagerComponent _cueManager;

        public void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _cueManager = GetComponentInParent<CueManagerComponent>();
            _cueManager.OnCueAdded += OnCueAdded;
        }

        public void OnCueAdded(string cueTag, CueDefinition definition)
        {
            if (!cueTag.StartsWith("Cue.Audio")) return;
            _audioSource.clip = definition.audioClip;
            Debug.Log("Playing audio");
            _audioSource.Play();
        }
    }
}