using System;
using AbilitySystem.Runtime.Tags;

namespace AbilitySystem.Runtime.Effects
{
 public struct EffectTags
    {
        public GameplayTagSet AssetTags;
        public GameplayTagSet GrantedTags;
        public GameplayTagSet RemoveEffectsWithTags;
        public GameplayTagSet ApplicationRequiredTags;
        public GameplayTagSet ApplicationImmunityTags;
        public GameplayTagSet OngoingRequiredTags;
        
        public EffectTags(
            GameplayTag[] assetTags,
            GameplayTag[] grantedTags,
            GameplayTag[] applicationRequiredTags,
            GameplayTag[] ongoingRequiredTags,
            GameplayTag[] removeGameplayEffectsWithTags,
            GameplayTag[] applicationImmunityTags)
        {
            AssetTags = new GameplayTagSet(assetTags);
            GrantedTags = new GameplayTagSet(grantedTags);
            ApplicationRequiredTags = new GameplayTagSet(applicationRequiredTags);
            OngoingRequiredTags = new GameplayTagSet(ongoingRequiredTags);
            RemoveEffectsWithTags = new GameplayTagSet(removeGameplayEffectsWithTags);
            ApplicationImmunityTags = new GameplayTagSet(applicationImmunityTags);
        }

        public EffectTags(EffectDefinition effectAsset)
        {
            AssetTags = new GameplayTagSet(effectAsset.assetTags);
            GrantedTags = new GameplayTagSet(effectAsset.grantedTags);
            ApplicationRequiredTags = new GameplayTagSet(effectAsset.applicationRequiredTags);
            OngoingRequiredTags = new GameplayTagSet(effectAsset.applicationRequiredTags);
            RemoveEffectsWithTags = new GameplayTagSet(effectAsset.removeGameplayEffectsWithTags);
            ApplicationImmunityTags = new GameplayTagSet(effectAsset.applicationRequiredTags);
        }
        
        public static EffectTags CreateEmpty()
        {
            return new EffectTags(
                Array.Empty<GameplayTag>(),
                Array.Empty<GameplayTag>(),
                Array.Empty<GameplayTag>(),
                Array.Empty<GameplayTag>(),
                Array.Empty<GameplayTag>(),
                Array.Empty<GameplayTag>()
            );
        }
    }
}