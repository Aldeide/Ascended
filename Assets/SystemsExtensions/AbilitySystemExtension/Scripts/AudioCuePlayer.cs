using AbilitySystem.Runtime.Cues;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    public class AudioCuePlayer : CueListenerComponent
    {
        [ShowInInspector] private AudioSource _audioSource;

        public override void Start()
        {
            base.Start();
            _audioSource = GetComponent<AudioSource>();

            // Automatically ensure SoundOcclusion is attached to the same GameObject
            if (GetComponent<Systems.Audio.SoundOcclusion>() == null)
            {
                gameObject.AddComponent<Systems.Audio.SoundOcclusion>();
            }
        }
        
        public override void OnExecuteCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
            _audioSource.clip = (definition as CueAudioDefinition)?.AudioClip;
            _audioSource.Play();
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
