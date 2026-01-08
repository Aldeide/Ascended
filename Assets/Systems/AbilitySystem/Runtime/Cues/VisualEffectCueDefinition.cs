using System;
using UnityEngine;
using UnityEngine.VFX;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    [CreateAssetMenu(fileName = "VisualEffectCue", menuName = "AbilitySystem/Cues/VisualEffectCue")]
    public class VisualEffectCueDefinition : CueDefinition
    {
        public VisualEffectAsset VisualEffectAsset;
    }
}