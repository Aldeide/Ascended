using System;
using UnityEngine;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    [CreateAssetMenu(fileName = "AudioCue", menuName = "AbilitySystem/AudioCue")]
    public class CueAudioDefinition : CueDefinition
    {
        public AudioClip AudioClip;
    }
}