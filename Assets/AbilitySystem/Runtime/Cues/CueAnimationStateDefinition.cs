using System;
using UnityEngine;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    [CreateAssetMenu(fileName = "AnimationStateCue", menuName = "AbilitySystem/AnimationStateCue")]
    public class CueAnimationStateDefinition : CueDefinition
    {
        [SerializeField]
        public string animationLayerName;
    }
}