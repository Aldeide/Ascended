using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.Tags
{
    public class GameplayTagManager
    {
        public List<GameplayTag> tags = new();
        private IAbilitySystem _owner;

        private event Action OnTagsChanged;

        public GameplayTagManager(IAbilitySystem owner)
        {
            _owner = owner;
        }

        public void AddTag(GameplayTag gameplayTag)
        {
            if (tags.Contains(gameplayTag)) return;
            tags.Add(gameplayTag);
            OnTagsChanged?.Invoke();
        }

        public void RemoveTag(GameplayTag gameplayTag)
        {
            tags.Remove(gameplayTag);
            OnTagsChanged?.Invoke();
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

        public void ApplyAbilityTags(Ability ability)
        {
            foreach (var tag in ability.Definition.AbilityTags.ActivationOwnedTag.Tags)
            {
                AddTag(tag);
            }
            OnTagsChanged?.Invoke();
        }
        
        public void RemoveAbilityTags(Ability ability)
        {
            foreach (var tag in ability.Definition.AbilityTags.ActivationOwnedTag.Tags)
            {
                RemoveTag(tag);
            }
            OnTagsChanged?.Invoke();
        }

        public string DebugString()
        {
            return tags.Aggregate("Tags\n", (current, tag) => current + (tag.GetName() + "\n"));
        }
    }
}