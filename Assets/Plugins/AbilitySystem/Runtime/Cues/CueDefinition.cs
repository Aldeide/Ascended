using System;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    [CreateAssetMenu(fileName = "Cue", menuName = "AbilitySystem/Cue")]
    public class CueDefinition : ScriptableObject
    {
        public VisualEffectAsset visualEffectAsset;
        public GameObject prefab;
        
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag cueTag;
    }
}