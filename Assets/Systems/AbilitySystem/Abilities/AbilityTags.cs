using System;
using Systems.AbilitySystem.Tags;

namespace Systems.AbilitySystem.Abilities
{
    [Serializable]
    public struct AbilityTags
    {
        public GameplayTagSet AssetTag;

        public GameplayTagSet CancelAbilitiesWithTags;
        public GameplayTagSet BlockAbilitiesWithTags;

        public GameplayTagSet ActivationOwnedTag;

        public GameplayTagSet ActivationRequiredTags;
        public GameplayTagSet ActivationBlockedTags;

        // // TODO
        // public GameplayTagSet SourceRequiredTags;
        // public GameplayTagSet SourceBlockedTags;
        //
        // // TODO
        // public GameplayTagSet TargetRequiredTags;
        // public GameplayTagSet TargetBlockedTags;

        public AbilityTags(
            GameplayTag[] assetTags,
            GameplayTag[] cancelAbilityTags,
            GameplayTag[] blockAbilityTags,
            GameplayTag[] activationOwnedTag,
            GameplayTag[] activationRequiredTags,
            GameplayTag[] activationBlockedTags)
        {
            AssetTag = new GameplayTagSet(assetTags);
            CancelAbilitiesWithTags = new GameplayTagSet(cancelAbilityTags);
            BlockAbilitiesWithTags = new GameplayTagSet(blockAbilityTags);
            ActivationOwnedTag = new GameplayTagSet(activationOwnedTag);
            ActivationRequiredTags = new GameplayTagSet(activationRequiredTags);
            ActivationBlockedTags = new GameplayTagSet(activationBlockedTags);
            // SourceRequiredTags = new GameplayTagSet(sourceRequiredTags);
            // SourceBlockedTags = new GameplayTagSet(sourceBlockedTags);
            // TargetRequiredTags = new GameplayTagSet(targetRequiredTags);
            // TargetBlockedTags = new GameplayTagSet(targetBlockedTags);
        }
    }
}