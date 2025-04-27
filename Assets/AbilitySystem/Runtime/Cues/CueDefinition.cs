using System;
using AbilitySystem.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    [CreateAssetMenu(fileName = "Cue", menuName = "AbilitySystem/Cue")]
    public class CueDefinition : ScriptableObject
    {
        public AudioClip audioClip;
        public VisualEffectAsset visualEffectAsset;
        public GameObject prefab;
        
        [ValueDropdown("@DropdownValuesUtil.CueTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag cueTag;
    }
}