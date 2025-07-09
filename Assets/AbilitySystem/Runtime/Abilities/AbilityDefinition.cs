using System;
using System.Linq;
using AbilitySystem.Runtime.Abilities.Cooldowns;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Runtime.Utilities;
using AbilitySystem.Scripts;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

namespace AbilitySystem.Runtime.Abilities
{
    [Serializable]
    public abstract class AbilityDefinition : ScriptableObject
    {
        public abstract Type AbilityType();

        public LocalizedString Description;
        public LocalizedString DisplayName;
        public Sprite Icon;

        [ShowInInspector]
        public string InstanceAbilityClassFullName => AbilityType() != null ? AbilityType().FullName : null;

        [ShowInInspector] public string TypeName => GetType().Name;
        [ShowInInspector] public string TypeFullName => GetType().FullName;
        [ShowInInspector] public string[] InheritanceChain => GetType().GetInheritanceChain().Reverse().ToArray();
        public string UniqueName;

        [Space]
        [Title("Cost and Cooldown")]
        public EffectDefinition Cost;
        [SerializeReference]
        public AbilityCooldown Cooldown;

        [SerializeReference]
        public AbilityActivation.AbilityActivation AbilityActivation;
        
        [Title("Tags")]
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] AssetTags;

        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] CancelAbilityTags;

        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] BlockAbilityTags;

        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] ActivationOwnedTags;

        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] ActivationRequiredTags;

        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] ActivationBlockedTags;

        [Space] [Title("Granted Effects")] public EffectDefinition[] GrantedEffects;

        [Space] [Title("Network")]
        public AbilityNetworkPolicy NetworkPolicy;

        public AbilityNetworkSecurityPolicy NetworkSecurityPolicy;
        
        [HideInInspector]
        public AbilityTags AbilityTags;

        [Space] [Title("Cues")] public CueDefinition[] ActivationCues;
        
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
            return NetworkPolicy == AbilityNetworkPolicy.ClientPredicted;
        }

        public bool IsLocalAbility()
        {
            return NetworkPolicy == AbilityNetworkPolicy.ClientOnly;
        }
    }
}