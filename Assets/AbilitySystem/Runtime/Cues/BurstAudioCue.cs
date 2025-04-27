using System;
using UnityEngine;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    public class BurstAudioCue : InstantCue
    {
        public AudioClip AudioClip;

        public BurstAudioCue(InstantCueDefinition definition) : base(definition)
        {
            AudioClip = definition.audioClip;
        }
        
        public override void Execute()
        {
            
        }
    }
}