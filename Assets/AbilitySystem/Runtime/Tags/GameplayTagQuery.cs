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
            return MatchesTags(new[] { tag });
        }

        public bool MatchesTags(GameplayTag[] tags)
        {
            switch (MatchType)
            {
                case GameplayTagMatchType.AnyOfExact when Tags.Any(t => tags.Any(tag => tag.Equals(t))):
                case GameplayTagMatchType.AllOfExact when Tags.All(t => tags.Any(tag => tag.Equals(t))):
                case GameplayTagMatchType.NoneOfExact when !Tags.Any(t => tags.Any(tag => tag.Equals(t))):
                case GameplayTagMatchType.AnyOfPartial when Tags.Any(t => tags.Any(tag => t.IsAncestorOf(tag))):
                case GameplayTagMatchType.AllOfPartial when Tags.All(t => tags.Any(tag => t.IsAncestorOf(tag))):
                case GameplayTagMatchType.NoneOfPartial when !Tags.Any(t => tags.Any(tag => t.IsAncestorOf(tag))):
                    return true;
                default:
                    return false;
            }
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