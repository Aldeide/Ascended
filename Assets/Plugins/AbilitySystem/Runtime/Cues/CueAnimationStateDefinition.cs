using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    [CreateAssetMenu(fileName = "AnimationStateCue", menuName = "AbilitySystem/AnimationStateCue")]
    public class CueAnimationStateDefinition : CueDefinition
    {
        [FormerlySerializedAs("animationLayerName")] [SerializeField]
        public string AnimationLayerName;
    }
}