using System;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AvatarSlots.Runtime
{
    // Allows specifying custom position and rotation for a slottable object based on the actual slot.
    [Serializable]
    public struct SlottableConfiguration
    {
        [ValueDropdown("@TagsDropdown.FilteredGameplayTagChoices(\"Slot.\")", IsUniqueList = true, HideChildProperties = true)]
        public Tag Slot;
        public Vector3 PositionOffset;
        public Quaternion RotationOffset;
    }
}