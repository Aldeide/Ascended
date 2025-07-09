using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameplayTags.Runtime
{
    [Serializable]
    public struct TagQuery
    {
        [SerializeField] public TagCondition[] Condition;

        public TagQuery(params TagCondition[] condition)
        {
            Condition = condition;
        }

        public bool MatchesTag(Tag tag)
        {
            return Condition.All(condition => condition.MatchesTag(tag));
        }

        public bool MatchesTags(Tag[] tags)
        {
            return Condition.All(condition => condition.MatchesTags(tags));
        }
    }

    [Serializable]
    public struct TagCondition
    {
        [SerializeField] public TagMatchType MatchType;

        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public Tag[] Tags;

        public TagCondition(TagMatchType matchType, params Tag[] tags)
        {
            MatchType = matchType;
            Tags = tags;
        }

        public bool MatchesTag(Tag tag)
        {
            return MatchesTags(new[] { tag });
        }

        public bool MatchesTags(Tag[] tags)
        {
            switch (MatchType)
            {
                case TagMatchType.AnyOfExact when Tags.Any(t => tags.Any(tag => tag.Equals(t))):
                case TagMatchType.AllOfExact when Tags.All(t => tags.Any(tag => tag.Equals(t))):
                case TagMatchType.NoneOfExact when !Tags.Any(t => tags.Any(tag => tag.Equals(t))):
                case TagMatchType.AnyOfPartial when Tags.Any(t => tags.Any(tag => t.IsAncestorOf(tag))):
                case TagMatchType.AllOfPartial when Tags.All(t => tags.Any(tag => t.IsAncestorOf(tag))):
                case TagMatchType.NoneOfPartial when !Tags.Any(t => tags.Any(tag => t.IsAncestorOf(tag))):
                    return true;
                default:
                    return false;
            }
        }
    }

    [Serializable]
    public enum TagMatchType
    {
        AnyOfExact,
        AllOfExact,
        NoneOfExact,
        AnyOfPartial,
        AllOfPartial,
        NoneOfPartial,
    }
}