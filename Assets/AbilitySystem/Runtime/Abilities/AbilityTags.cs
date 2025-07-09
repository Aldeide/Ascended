using System;
using AbilitySystem.Runtime.Tags;
using GameplayTags.Runtime;

namespace AbilitySystem.Runtime.Abilities
{
    [Serializable]
    public struct AbilityTags
    {
        public TagSet AssetTag;

        public TagSet CancelAbilitiesWithTags;
        public TagSet BlockAbilitiesWithTags;

        public TagSet ActivationOwnedTag;

        public TagSet ActivationRequiredTags;
        public TagSet ActivationBlockedTags;

        // // TODO
        // public GameplayTagSet SourceRequiredTags;
        // public GameplayTagSet SourceBlockedTags;
        //
        // // TODO
        // public GameplayTagSet TargetRequiredTags;
        // public GameplayTagSet TargetBlockedTags;

        public AbilityTags(
            Tag[] assetTags,
            Tag[] cancelAbilityTags,
            Tag[] blockAbilityTags,
            Tag[] activationOwnedTag,
            Tag[] activationRequiredTags,
            Tag[] activationBlockedTags)
        {
            AssetTag = new TagSet(assetTags);
            CancelAbilitiesWithTags = new TagSet(cancelAbilityTags);
            BlockAbilitiesWithTags = new TagSet(blockAbilityTags);
            ActivationOwnedTag = new TagSet(activationOwnedTag);
            ActivationRequiredTags = new TagSet(activationRequiredTags);
            ActivationBlockedTags = new TagSet(activationBlockedTags);
            // SourceRequiredTags = new GameplayTagSet(sourceRequiredTags);
            // SourceBlockedTags = new GameplayTagSet(sourceBlockedTags);
            // TargetRequiredTags = new GameplayTagSet(targetRequiredTags);
            // TargetBlockedTags = new GameplayTagSet(targetBlockedTags);
        }

        public static AbilityTags CreateEmpty()
        {
            return new AbilityTags
            (Array.Empty<Tag>(), Array.Empty<Tag>(), Array.Empty<Tag>(),
                Array.Empty<Tag>(), Array.Empty<Tag>(), Array.Empty<Tag>()
            );
        }
    }
}