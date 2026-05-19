using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Tags
{
    [TestFixture]
    public class GameplayTagManagerTests : AbilitySystemTestBase
    {
        [Test]
        public void GameplayTagManager_HasPartialTag_MatchesDescendants()
        {
            var parent = new Tag("State.Debuff");
            var childInherent = new Tag("State.Debuff.Stun");
            var childEffect = new Tag("State.Debuff.Freeze");
            var childAbility = new Tag("State.Debuff.Silence");

            // 1. Inherent tag check
            Source.TagManager.AddTag(childInherent);
            Assert.IsTrue(Source.TagManager.HasPartialTag(parent), "Should match inherent descendant tag");
            Source.TagManager.RemoveTag(childInherent);
            Assert.IsFalse(Source.TagManager.HasPartialTag(parent), "Should not match after removal");

            // 2. Ability tag check
            var syncData = new AbilityTagSyncData
            {
                AbilityUniqueName = "SilenceAbility",
                Tags = new[] { childAbility }
            };
            Source.TagManager.AddAbilityTags(syncData);
            Assert.IsTrue(Source.TagManager.HasPartialTag(parent), "Should match ability descendant tag");
            Source.TagManager.RemoveAbilityTags(syncData);
            Assert.IsFalse(Source.TagManager.HasPartialTag(parent), "Should not match after ability tag removal");

            // 3. Effect tag check
            var effect = EffectUtilities.CreateDurationalEffectWithTag(Source, Source);
            // This effect grants "Tag.Test.GrantedTag"
            Source.EffectManager.AddEffectFromServer(effect);
            Assert.IsTrue(Source.TagManager.HasPartialTag(new Tag("Tag.Test")), "Should match effect descendant tag");
            Source.EffectManager.RemoveEffect(effect);
            Assert.IsFalse(Source.TagManager.HasPartialTag(new Tag("Tag.Test")), "Should not match after effect removal");
        }

        [Test]
        public void GameplayTagManager_HasAllTags_EvaluatesCorrectly()
        {
            var tagA = new Tag("Tag.A");
            var tagB = new Tag("Tag.B");
            var tagC = new Tag("Tag.C");

            Source.TagManager.AddTag(tagA);
            Source.TagManager.AddTag(tagB);

            // Array overload
            Assert.IsTrue(Source.TagManager.HasAllTags(new[] { tagA, tagB }));
            Assert.IsFalse(Source.TagManager.HasAllTags(new[] { tagA, tagB, tagC }));

            // TagSet overload
            var tagSetTrue = new TagSet(tagA, tagB);
            var tagSetFalse = new TagSet(tagA, tagB, tagC);
            Assert.IsTrue(Source.TagManager.HasAllTags(tagSetTrue));
            Assert.IsFalse(Source.TagManager.HasAllTags(tagSetFalse));
        }

        [Test]
        public void GameplayTagManager_HasAnyTags_EvaluatesCorrectly()
        {
            var tagA = new Tag("Tag.A");
            var tagB = new Tag("Tag.B");
            var tagC = new Tag("Tag.C");

            Source.TagManager.AddTag(tagA);

            Assert.IsTrue(Source.TagManager.HasAnyTags(tagA, tagB));
            Assert.IsTrue(Source.TagManager.HasAnyTags(tagC, tagA));
            Assert.IsFalse(Source.TagManager.HasAnyTags(tagB, tagC));
        }

        [Test]
        public void GameplayTagManager_HasAnyPartialTag_EvaluatesCorrectly()
        {
            var parent = new Tag("State.Debuff");
            var child = new Tag("State.Debuff.Stun");
            var unrelated = new Tag("State.Buff");

            Source.TagManager.AddTag(child);

            Assert.IsTrue(Source.TagManager.HasAnyPartialTag(parent, unrelated), "Should match parent of active child tag");
            Assert.IsFalse(Source.TagManager.HasAnyPartialTag(unrelated), "Should not match unrelated tags");
        }
    }
}
