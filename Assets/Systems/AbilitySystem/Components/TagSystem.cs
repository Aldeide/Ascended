using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using Systems.AbilitySystem.Abilities;
using Systems.AbilitySystem.Tags;
using UnityEngine.Serialization;

namespace Systems.AbilitySystem.Components
{
    public class TagSystem : NetworkBehaviour
    {
        public List<GameplayTag> tags = new();
        private AbilitySystemComponent _asc;

        private event Action OnTagsChanged;
        
        public void Initialise(AbilitySystemComponent owner)
        {
            _asc = owner;
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

        public void ApplyAbilityTags(AbilitySpec ability)
        {
            foreach (var tag in ability.Ability.AbilityTags.ActivationOwnedTag.Tags)
            {
                AddTag(tag);
            }
            OnTagsChanged?.Invoke();
        }
        
        public void RemoveAbilityTags(AbilitySpec ability)
        {
            foreach (var tag in ability.Ability.AbilityTags.ActivationOwnedTag.Tags)
            {
                RemoveTag(tag);
            }
            OnTagsChanged?.Invoke();
        }
    }
}