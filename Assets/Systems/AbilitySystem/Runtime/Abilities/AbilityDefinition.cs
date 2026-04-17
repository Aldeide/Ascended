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

        public LocalizedString Description;
        public LocalizedString DisplayName;
        public Sprite Icon;

        [Sirenix.OdinInspector.ShowInInspector]
        public string InstanceAbilityClassFullName => AbilityType() != null ? AbilityType().FullName : null;

        [Sirenix.OdinInspector.ShowInInspector] public string TypeName => GetType().Name;
        [Sirenix.OdinInspector.ShowInInspector] public string TypeFullName => GetType().FullName;
        [Sirenix.OdinInspector.ShowInInspector] public string[] InheritanceChain => GetType().GetInheritanceChain().Reverse().ToArray();
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