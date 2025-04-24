using System;
using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Runtime.Utilities;
using AbilitySystem.Scripts;
using Sirenix.OdinInspector;
using UnityEditor.VersionControl;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    [Serializable]
    public abstract class AbilityDefinition : ScriptableObject
    {
        public abstract Type AbilityType();

        public string description;
        public Sprite icon;

        [ShowInInspector]
        public string InstanceAbilityClassFullName => AbilityType() != null ? AbilityType().FullName : null;

        [ShowInInspector] public string TypeName => GetType().Name;
        [ShowInInspector] public string TypeFullName => GetType().FullName;
        [ShowInInspector] public string[] InheritanceChain => GetType().GetInheritanceChain().Reverse().ToArray();
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

        [Space] [Title("Granted Effects")] public EffectDefinition[] grantedEffects;

        [Space] [Title("Prediction")] public AbilityNetworkPolicy networkPolicy;
        public AbilityTags AbilityTags;

        public AbilityDefinition()
        {
            AbilityTags = new AbilityTags(
                AssetTags, CancelAbilityTags, BlockAbilityTags, ActivationOwnedTags,
                ActivationRequiredTags, ActivationBlockedTags
            );
        }

        public abstract Ability ToAbility(IAbilitySystem owner);

        public bool HasLocalPrediction()
        {
            return networkPolicy == AbilityNetworkPolicy.ClientPredicted;
        }

        public bool IsLocalAbility()
        {
            return networkPolicy == AbilityNetworkPolicy.ClientOnly;
        }
    }
}