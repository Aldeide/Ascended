using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Systems.AbilitySystem.Tags;
using Systems.AbilitySystem.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.AbilitySystem.Authoring
{
    public abstract class AbilityAsset : ScriptableObject
    {
        private static IEnumerable _abilityClassChoice = new ValueDropdownList<string>();
        public abstract Type AbilityType();
        
        public string description;
        public Sprite icon;
        public string InstanceAbilityClassFullName => AbilityType() != null ? AbilityType().FullName : null;
        public string TypeName => GetType().Name;
        public string TypeFullName => GetType().FullName;
        public string[] InheritanceChain => GetType().GetInheritanceChain().Reverse().ToArray();
        public string uniqueName;

        public EffectAsset cost;
        
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] AssetTags;
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] CancelAbilityTags;
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] BlockAbilityTags;
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] ActivationOwnedTags;
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] ActivationRequiredTags;
        [ValueDropdown("@ValueDropdownUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] ActivationBlockedTags;
        
    }
}