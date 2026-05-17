using System;
using System.Linq;
using AbilitySystem.Runtime.Abilities.Cooldowns;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Utilities;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Localization;

namespace AbilitySystem.Runtime.Abilities
{
    [Serializable]
    public abstract class AbilityDefinition : BaseGraph
    {
        public abstract Type AbilityType();

        [TabGroup("tab1", "General", SdfIconType.ImageAlt, TextColor = "green")]
        public string UniqueName;
        [TabGroup("tab1", "General", SdfIconType.ImageAlt, TextColor = "green")]
        public LocalizedString Description;
        [TabGroup("tab1", "General", SdfIconType.ImageAlt, TextColor = "green")]
        public LocalizedString DisplayName;
        [TabGroup("tab1", "General", SdfIconType.ImageAlt, TextColor = "green")]
        public Sprite Icon;

        [Sirenix.OdinInspector.ShowInInspector]
        public string InstanceAbilityClassFullName => AbilityType() != null ? AbilityType().FullName : null;

        [Sirenix.OdinInspector.ShowInInspector] public string TypeName => GetType().Name;
        [Sirenix.OdinInspector.ShowInInspector] public string TypeFullName => GetType().FullName;
        
        
        [Sirenix.OdinInspector.ShowInInspector] public string[] InheritanceChain => GetType().GetInheritanceChain().Reverse().ToArray();
        

        [Space]
        [Title("Cost and Cooldown")]
        [TabGroup("tab1", "General", SdfIconType.ImageAlt, TextColor = "green")]
        public EffectDefinition Cost;
        [SerializeReference]
        [TabGroup("tab1", "General", SdfIconType.ImageAlt, TextColor = "green")]
        public AbilityCooldown Cooldown;

        [SerializeReference]
        [TabGroup("tab1", "General", SdfIconType.ImageAlt, TextColor = "green")]
        public AbilityActivation.AbilityActivation AbilityActivation;
        
        [TabGroup("tab1", "Tags", SdfIconType.ImageAlt, TextColor = "blue")]
        [Title("Tags")]
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] AssetTags = Array.Empty<Tag>();

        [TabGroup("tab1", "Tags", SdfIconType.ImageAlt, TextColor = "blue")]
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] CancelAbilityTags = Array.Empty<Tag>();

        [TabGroup("tab1", "Tags", SdfIconType.ImageAlt, TextColor = "blue")]
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] BlockAbilityTags = Array.Empty<Tag>();

        [TabGroup("tab1", "Tags", SdfIconType.ImageAlt, TextColor = "blue")]
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] ActivationOwnedTags = Array.Empty<Tag>();

        [TabGroup("tab1", "Tags", SdfIconType.ImageAlt, TextColor = "blue")]
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] ActivationRequiredTags = Array.Empty<Tag>();

        [TabGroup("tab1", "Tags", SdfIconType.ImageAlt, TextColor = "blue")]
        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] ActivationBlockedTags = Array.Empty<Tag>();

        [Space] [Title("Granted Effects")] public EffectDefinition[] GrantedEffects = Array.Empty<EffectDefinition>();

        [Space] [Title("Network")]
        public AbilityNetworkPolicy NetworkPolicy;

        public AbilityNetworkSecurityPolicy NetworkSecurityPolicy;
        
        [HideInInspector]
        public AbilityTags AbilityTags;

        [Space] [Title("Cues")] public CueDefinition[] ActivationCues = Array.Empty<CueDefinition>();
        
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