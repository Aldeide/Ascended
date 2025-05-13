using System;
using System.Linq;

namespace AbilitySystem.Runtime.Tags
{
    [Serializable]
    public struct GameplayTagQuery
    {
        public GameplayTagCondition[] Condition;

        public GameplayTagQuery(params GameplayTagCondition[] condition)
        {
            Condition = condition;
        }
        
        public bool MatchesTag(GameplayTag tag)
        {
            return Condition.All(condition => condition.MatchesTag(tag));
        }
    }

    [Serializable]
    public struct GameplayTagCondition
    {
        public GameplayTagMatchType MatchType;
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