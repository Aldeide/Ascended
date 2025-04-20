using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using UnityEngine;

namespace AbilitySystem.Runtime.Tags
{
    public class GameplayTagManager
    {
        public List<GameplayTag> Tags = new();
        public Dictionary<GameplayTag, List<Effect>> EffectTags = new();
        private IAbilitySystem _owner;

        private event Action OnTagsChanged;

        public GameplayTagManager(IAbilitySystem owner)
        {
            _owner = owner;
            _owner.EffectManager.OnEffectAdded += RefreshTags;
            _owner.EffectManager.OnEffectRemoved += RefreshTags;
        }

        public void RefreshTags(Effect e)
        {
            EffectTags.Clear();
            var effects = _owner.EffectManager.GetActiveEffects();
            foreach (var effect in effects)
            {
                AddEffectTags(effect);
            }
        }

        public void AddEffectTags(Effect effect)
        {
            foreach (var tag in effect.Definition.grantedTags)
            {
                if (EffectTags.ContainsKey(tag))
                {
                    EffectTags[tag].Add(effect);
                    continue;
                }

                EffectTags[tag] = new List<Effect> { effect };
            }
        }

        public void RemoveEffectTags(Effect effect)
        {
            foreach (var tag in effect.Definition.grantedTags)
            {
                if (EffectTags.ContainsKey(tag))
                {
                    EffectTags[tag].Remove(effect);
                }

                if (EffectTags[tag].Count == 0)
                {
                    EffectTags.Remove(tag);
                }
            }
        }

        public void AddTag(GameplayTag gameplayTag)
        {
            if (Tags.Contains(gameplayTag)) return;
            Tags.Add(gameplayTag);
            OnTagsChanged?.Invoke();
        }

        public void RemoveTag(GameplayTag gameplayTag)
        {
            Tags.Remove(gameplayTag);
            OnTagsChanged?.Invoke();
        }

        public bool HasTag(GameplayTag gameplayTag)
        {
            return Tags.Contains(gameplayTag) || EffectTags.ContainsKey(gameplayTag);
        }

        public bool HasPartialTag(GameplayTag gameplayTag)
        {
            return Tags.Any(tag => gameplayTag.IsAncestorOf(tag)) ||
                   EffectTags.Keys.Any(tag => gameplayTag.IsAncestorOf(tag));
        }

        public bool HasAllTags(GameplayTagSet gameplayTags)
        {
            return gameplayTags.Tags.All(HasTag);
        }

        public bool HasAnyTags(params GameplayTag[] gameplayTags)
        {
            return gameplayTags.Any(HasTag);
        }

        public bool HasAnyPartialTag(params GameplayTag[] gameplayTags)
        {
            return gameplayTags.Any(HasTag) || gameplayTags.Any(HasPartialTag);
        }

        public void ApplyAbilityTags(Ability ability)
        {
            Debug.Log("Applying tags");
            foreach (var tag in ability.Definition.ActivationOwnedTags)
            {
                Debug.Log(tag.GetName());
                AddTag(tag);
            }

            OnTagsChanged?.Invoke();
        }

        public void RemoveAbilityTags(Ability ability)
        {
            foreach (var tag in ability.Definition.ActivationOwnedTags)
            {
                RemoveTag(tag);
            }

            OnTagsChanged?.Invoke();
        }

        public string DebugString()
        {
            var inherentTags = Tags.Aggregate("Inherent Tags\n", (current, tag) => current + (tag.GetName() + "\n"));
            var effectTags = "Effect Tags\n";
            foreach (var tag in EffectTags)
            {
                effectTags += tag.Key.GetName() + " (";
                effectTags = tag.Value.Aggregate(effectTags,
                    (current, effect) => current + (effect.Definition.name + " "));
                effectTags += ")\n";
            }

            return inherentTags + "\n" + effectTags;
        }
    }
}