using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;

namespace AbilitySystem.Runtime.Tags
{
    public class GameplayTagManager
    {
        // Inherent tags (e.g. Unit.Player)
        public List<GameplayTag> Tags = new();
        // Tags granted while effects are active.
        public Dictionary<GameplayTag, List<Effect>> EffectTags = new();
        // Tags granted while abilities are active.
        public Dictionary<GameplayTag, List<Ability>> AbilityTags = new();
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

        public void AddAbilityTags(Ability ability)
        {
            foreach (var tag in ability.Definition.ActivationOwnedTags)
            {
                if (AbilityTags.ContainsKey(tag))
                {
                    AbilityTags[tag].Add(ability);
                    continue;
                }

                AbilityTags[tag] = new List<Ability> { ability };
            }
            OnTagsChanged?.Invoke();
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

        public void RemoveAbilityTags(Ability ability)
        {
            foreach (var tag in ability.Definition.ActivationOwnedTags)
            {
                if (AbilityTags.ContainsKey(tag))
                {
                    AbilityTags[tag].Remove(ability);
                    OnTagsChanged?.Invoke();
                }

                if (AbilityTags[tag].Count == 0)
                {
                    AbilityTags.Remove(tag);
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
            return Tags.Contains(gameplayTag) || EffectTags.ContainsKey(gameplayTag) ||
                   AbilityTags.ContainsKey(gameplayTag);
        }

        public bool HasPartialTag(GameplayTag gameplayTag)
        {
            return Tags.Any(tag => gameplayTag.IsAncestorOf(tag)) ||
                   EffectTags.Keys.Any(tag => gameplayTag.IsAncestorOf(tag)) ||
                   AbilityTags.Keys.Any(tag => gameplayTag.IsAncestorOf(tag));;
        }

        public bool HasAllTags(GameplayTagSet gameplayTags)
        {
            return gameplayTags.Tags.All(HasTag);
        }

        public bool HasAllTags(GameplayTag[] gameplayTags)
        {
            return gameplayTags.All(HasTag);
        }

        public bool HasAnyTags(params GameplayTag[] gameplayTags)
        {
            return gameplayTags.Any(HasTag);
        }

        public bool HasAnyPartialTag(params GameplayTag[] gameplayTags)
        {
            return gameplayTags.Any(HasTag) || gameplayTags.Any(HasPartialTag);
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
            var abilityTags = "Ability Tags\n";
            foreach (var tag in AbilityTags)
            {
                abilityTags += tag.Key.GetName() + " (";
                abilityTags = tag.Value.Aggregate(abilityTags,
                    (current, ability) => current + (ability.Definition.name + " "));
                abilityTags += ")\n";
            }
            return inherentTags + "\n" + effectTags + "\n" + abilityTags + "\n";
        }
    }
}