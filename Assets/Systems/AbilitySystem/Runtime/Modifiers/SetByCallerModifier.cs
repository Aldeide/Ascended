using System;
using AbilitySystem.Runtime.Effects;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public class SetByCallerModifier : Modifier
    {
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag DataTag;

        public override float Calculate(Effect effect)
        {
            return effect.GetSetByCallerMagnitude(DataTag);
        }
    }
}
