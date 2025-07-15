using System;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    public class CueDefinition : ScriptableObject
    {
        [FormerlySerializedAs("cueTag")] [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag CueTag;
    }
}