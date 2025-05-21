using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Runtime.Tags
{
    [Serializable]
    public struct GameplayTagQuery
    {
        [SerializeField]
        public GameplayTagCondition[] Condition;

        public GameplayTagQuery(params GameplayTagCondition[] condition)
        {
            Condition = condition;
        }
        
        public bool MatchesTag(GameplayTag tag)
        {
            return Condition.All(condition => condition.MatchesTag(tag));
        }
        
        public bool MatchesTags(GameplayTag[] tags)
        {
            return Condition.All(condition => condition.MatchesTags(tags));
        }
    }

    [Serializable]
    public struct GameplayTagCondition
    {
        [SerializeField]
        public GameplayTagMatchType MatchType;
        [SerializeField]
        [ValueDropdown("@DropdownValuesUtil.AllTags", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] Tags;

        public GameplayTagCondition(GameplayTagMatchType matchType, params GameplayTag[] tags)
        {
            MatchType = matchType;
            Tags = tags;
        }
        
        public bool MatchesTag(GameplayTag tag)
        {
            if (MatchType == GameplayTagMatchType.AnyOfExact && Tags.Any(t => t.Equals(tag))) return true;
            if (MatchType == GameplayTagMatchType.AllOfExact && Tags.All(t => t.Equals(tag))) return true;
            if (MatchType == GameplayTagMatchType.NoneOfExact && !Tags.Any(t => t.Equals(tag))) return true;
            if (MatchType == GameplayTagMatchType.AnyOfPartial && Tags.Any(t => t.HasTag(tag))) return true;
            if (MatchType == GameplayTagMatchType.AllOfPartial && Tags.All(t => t.HasTag(tag))) return true;
            if (MatchType == GameplayTagMatchType.NoneOfPartial && !Tags.Any(t => t.HasTag(tag))) return true;
            return false;
        }

        public bool MatchesTags(GameplayTag[] tags)
        {
            if (MatchType == GameplayTagMatchType.AnyOfExact && Tags.Any(t => tags.Any(tag => tag.Equals(t)))) return true;
            if (MatchType == GameplayTagMatchType.AllOfExact && Tags.All(t => tags.Any(tag => tag.Equals(t)))) return true;
            if (MatchType == GameplayTagMatchType.NoneOfExact && !Tags.Any(t => tags.Any(tag => tag.Equals(t)))) return true;
            if (MatchType == GameplayTagMatchType.AnyOfPartial && Tags.Any(t => tags.Any(tag => t.HasTag(tag)))) return true;
            if (MatchType == GameplayTagMatchType.AllOfPartial && Tags.All(t => tags.Any(tag => t.HasTag(tag)))) return true;
            if (MatchType == GameplayTagMatchType.NoneOfPartial && !Tags.Any(t => tags.Any(tag => t.HasTag(tag)))) return true;
            return false;
        }
    }

    [Serializable]
    public enum GameplayTagMatchType
    {
        AnyOfExact,
        AllOfExact,
        NoneOfExact,
        AnyOfPartial,
        AllOfPartial,
        NoneOfPartial,
    }
}