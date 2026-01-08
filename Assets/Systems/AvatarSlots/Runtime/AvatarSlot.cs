using System;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Plugins.AvatarSlots.Runtime
{
    [Serializable]
    public struct AvatarSlot
    {
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag SlotTag;
        public Transform Transform;
    }
}