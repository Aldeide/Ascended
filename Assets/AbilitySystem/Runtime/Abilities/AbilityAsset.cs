using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Runtime.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
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
        [Title("Tags")]
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] AssetTags;
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] CancelAbilityTags;
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] BlockAbilityTags;
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] ActivationOwnedTags;
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] ActivationRequiredTags;
        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] ActivationBlockedTags;

        [Space] [Title("Granted Effects")] public EffectAsset[] grantedEffects; 
        
        [Space] [Title("Prediction")] public AbilityNetworkPolicy networkPolicy;
    }
}