using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using Systems.AbilitySystem.Tags;
using UnityEngine.Serialization;

namespace Systems.AbilitySystem.Components
{
    public class TagSystem
    {
        public List<GameplayTag> tags = new();
        private AbilitySystemComponent _asc;

        public TagSystem(AbilitySystemComponent owner)
        {
            _asc = owner;
        }

        public void AddTag(GameplayTag gameplayTag)
        {
            if (tags.Contains(gameplayTag)) return;
            tags.Add(gameplayTag);
        }

        public void RemoveTag(GameplayTag gameplayTag)
        {
            tags.Remove(gameplayTag);
        }

        public bool HasTag(GameplayTag gameplayTag)
        {
            return tags.Contains(gameplayTag);
        }

        public bool HasAllTags(GameplayTagSet gameplayTags)
        {
            return gameplayTags.Tags.All(HasTag);
        }
        
        public bool HasAnyTags(params GameplayTag[] gameplayTags)
        {
            return gameplayTags.Any(HasTag);
        }
    }
}