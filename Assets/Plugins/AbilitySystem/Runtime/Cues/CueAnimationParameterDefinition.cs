using System;
using UnityEngine;

namespace AbilitySystem.Runtime.Cues
{
    /// <summary>
    /// Defines the parameters for an animation-based parameter cue within the ability system.
    /// </summary>
    /// <remarks>
    /// A cue is a scriptable object used to represent a visual or functional feedback
    /// in the ability system during gameplay.
    /// CueAnimationParameterDefinition extends the base CueDefinition class and adds
    /// the ability to specify an animation parameter to trigger.
    /// </remarks>
    [Serializable]
    [CreateAssetMenu(fileName = "AnimationStateCue", menuName = "AbilitySystem/AnimationParameterCue")]
    public class CueAnimationParameterDefinition : CueDefinition
    {
        public string ParameterName;
    }
}