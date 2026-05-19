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

        public static TagQuery Empty => new TagQuery { Condition = Array.Empty<TagCondition>() };

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
            if (Condition == null || Condition.Length == 0) return true;
            if (tags == null) return false;
            return Condition.All(condition => condition.MatchesTags(tags));
        }
    }

    [Serializable]
    public struct TagCondition
    {
        [SerializeField] public TagMatchType MatchType;

        [ValueDropdown("@TagsDropdown.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [SerializeField]
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
            if (Tags == null || tags == null)
            {
                return MatchType == TagMatchType.NoneOfExact || MatchType == TagMatchType.NoneOfPartial;
            }
            // Does this need other types? AnyAncestorOf, AllAncestorOf?
            switch (MatchType)
            {
                case TagMatchType.AnyOfExact:
                    return Tags.Any(t => tags.Any(tag => tag.Equals(t)));
                case TagMatchType.AllOfExact:
                    return Tags.All(t => tags.Any(tag => tag.Equals(t)));
                case TagMatchType.NoneOfExact:
                    return !Tags.Any(t => tags.Any(tag => tag.Equals(t)));
                case TagMatchType.AnyOfPartial:
                    return Tags.Any(t => tags.Any(tag => tag.Equals(t) || t.IsAncestorOf(tag)));
                case TagMatchType.AllOfPartial:
                    return Tags.All(t => tags.Any(tag => tag.Equals(t) || t.IsAncestorOf(tag)));
                case TagMatchType.NoneOfPartial:
                    return !Tags.Any(t => tags.Any(tag => tag.Equals(t) || t.IsAncestorOf(tag)));
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