using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Scripts;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystem.Runtime.Tags
{
    public class GameplayTagManager
    {
        // Inherent tags (e.g. Unit.Player)
        public List<Tag> Tags = new();
        // Tags granted while effects are active.
        public Dictionary<Tag, List<Effect>> EffectTags = new();
        // Tags granted while abilities are active.
        public Dictionary<Tag, List<string>> AbilityTags = new();
        // Tags that are currently preventing abilities with matching asset tags from activating.
        public Dictionary<Tag, List<string>> BlockedAbilityTags = new();
        private IAbilitySystem _owner;

        public event Action OnTagsChanged;

        public GameplayTagManager(IAbilitySystem owner)
        {
            _owner = owner;
            _owner.EffectManager.OnEffectAdded += RefreshTags;
            _owner.EffectManager.OnEffectRemoved += RefreshTags;
            _owner.EffectManager.OnEffectSuspended += RefreshTags;
            _owner.EffectManager.OnEffectResumed += RefreshTags;
            _owner.EffectManager.OnEffectRetracted += RefreshTags;
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
            if (effect.Definition.GrantedTags == null || effect.Definition.GrantedTags.Length == 0) return;
            foreach (var tag in effect.Definition.GrantedTags)
            {
                if (EffectTags.ContainsKey(tag))
                {
                    EffectTags[tag].Add(effect);
                }
                else
                {
                    EffectTags.Add(tag, new List<Effect> { effect });
                }
            }
            OnTagsChanged?.Invoke();
        }

        public void AddAbilityTags(Ability ability)
        {
            if (ability.Definition.ActivationOwnedTags == null) return;
            foreach (var tag in ability.Definition.ActivationOwnedTags)
            {
                if (AbilityTags.ContainsKey(tag))
                {
                    AbilityTags[tag].Add(ability.Definition.UniqueName);
                    continue;
                }

                AbilityTags[tag] = new List<string> { ability.Definition.UniqueName };
            }
            OnTagsChanged?.Invoke();
        }
        
        public void AddAbilityTags(AbilityTagSyncData abilityTags)
        {
            foreach (var tag in abilityTags.Tags)
            {
                if (AbilityTags.ContainsKey(tag))
                {
                    AbilityTags[tag].Add(abilityTags.AbilityUniqueName);
                    continue;
                }

                AbilityTags[tag] = new List<string> { abilityTags.AbilityUniqueName };
            }
            OnTagsChanged?.Invoke();
        }

        public void AddAbilityTagsAndNotify(Ability ability)
        {
            if (!_owner.IsServer()) return;
            AddAbilityTags(ability);
            var abilityTags = new AbilityTagSyncData
            {
                AbilityUniqueName = ability.Definition.UniqueName,
                Tags = ability.Definition.ActivationOwnedTags
            };
            _owner.ReplicationManager.NotifyClientsAbilityTagsAdded(abilityTags);
        }
        
        public void RemoveAbilityTagsAndNotify(Ability ability)
        {
            if (!_owner.IsServer()) return;
            RemoveAbilityTags(ability);
            var abilityTags = new AbilityTagSyncData
            {
                AbilityUniqueName = ability.Definition.UniqueName,
                Tags = ability.Definition.ActivationOwnedTags
            };
            _owner.ReplicationManager.NotifyClientsAbilityTagsRemoved(abilityTags);
        }

        public void RemoveEffectTags(Effect effect)
        {
            if (effect.Definition.GrantedTags == null) return;
            foreach (var tag in effect.Definition.GrantedTags)
            {
                if (EffectTags.ContainsKey(tag))
                {
                    EffectTags[tag].Remove(effect);
                }

                if (EffectTags.ContainsKey(tag) && EffectTags[tag].Count == 0)
                {
                    EffectTags.Remove(tag);
                }
            }
        }

        public void RemoveAbilityTags(Ability ability)
        {
            if (ability.Definition.ActivationOwnedTags == null) return;
            foreach (var tag in ability.Definition.ActivationOwnedTags)
            {
                if (AbilityTags.ContainsKey(tag))
                {
                    AbilityTags[tag].Remove(ability.Definition.UniqueName);
                    OnTagsChanged?.Invoke();
                }

                if (AbilityTags.ContainsKey(tag) && AbilityTags[tag].Count == 0)
                {
                    AbilityTags.Remove(tag);
                }
            }
        }
        
        public void RemoveAbilityTags(AbilityTagSyncData abilityTags)
        {
            if (abilityTags.Tags == null) return;
            var tags = abilityTags.Tags;
            foreach (var tag in tags)
            {
                if (AbilityTags.ContainsKey(tag))
                {
                    AbilityTags[tag].Remove(abilityTags.AbilityUniqueName);
                    OnTagsChanged?.Invoke();
                }

                if (AbilityTags.ContainsKey(tag) && AbilityTags[tag].Count == 0)
                {
                    AbilityTags.Remove(tag);
                }
            }
        }

        public void AddAbilityBlockingTags(Ability ability)
        {
            if (ability.Definition.BlockAbilityTags == null) return;
            foreach (var tag in ability.Definition.BlockAbilityTags)
            {
                if (BlockedAbilityTags.ContainsKey(tag))
                {
                    BlockedAbilityTags[tag].Add(ability.Definition.UniqueName);
                    continue;
                }

                BlockedAbilityTags[tag] = new List<string> { ability.Definition.UniqueName };
            }
        }

        public void RemoveAbilityBlockingTags(Ability ability)
        {
            if (ability.Definition.BlockAbilityTags == null) return;
            foreach (var tag in ability.Definition.BlockAbilityTags)
            {
                if (BlockedAbilityTags.ContainsKey(tag))
                {
                    BlockedAbilityTags[tag].Remove(ability.Definition.UniqueName);
                }

                if (BlockedAbilityTags.ContainsKey(tag) && BlockedAbilityTags[tag].Count == 0)
                {
                    BlockedAbilityTags.Remove(tag);
                }
            }
        }

        public bool IsAbilityBlocked(Tag[] abilityTags)
        {
            if (abilityTags == null || abilityTags.Length == 0) return false;
            
            // Check if any of the ability's tags (or their parents) are currently blocked.
            return abilityTags.Any(tag => 
                BlockedAbilityTags.Keys.Any(blockedTag => blockedTag == tag || blockedTag.IsAncestorOf(tag)));
        }

        public void AddTag(Tag gameplayTag)
        {
            if (Tags.Contains(gameplayTag)) return;
            Tags.Add(gameplayTag);
            if (_owner.IsServer())
            {
                _owner.ReplicationManager.NotifyClientsTagAdded(gameplayTag);
            }
            OnTagsChanged?.Invoke();
        }

        public void RemoveTag(Tag gameplayTag)
        {
            if (!Tags.Contains(gameplayTag)) return;
            Tags.Remove(gameplayTag);
            if (_owner.IsServer())
            {
                _owner.ReplicationManager.NotifyClientsTagRemoved(gameplayTag);
            }
            OnTagsChanged?.Invoke();
        }

        public void RemoveAbilityTag(Tag gameplayTag)
        {
            AbilityTags.Remove(gameplayTag);
            OnTagsChanged?.Invoke();
        }

        public virtual bool HasTag(Tag gameplayTag)
        {
            return Tags.Contains(gameplayTag) || EffectTags.ContainsKey(gameplayTag) ||
                   AbilityTags.ContainsKey(gameplayTag);
        }

        public bool HasPartialTag(Tag gameplayTag)
        {
            return Tags.Any(tag => gameplayTag.IsAncestorOf(tag)) ||
                   EffectTags.Keys.Any(tag => gameplayTag.IsAncestorOf(tag)) ||
                   AbilityTags.Keys.Any(tag => gameplayTag.IsAncestorOf(tag));;
        }

        public bool HasAllTags(TagSet gameplayTags)
        {
            return gameplayTags.Tags.All(HasTag);
        }

        public bool HasAllTags(Tag[] gameplayTags)
        {
            return gameplayTags.All(HasTag);
        }

        public bool HasAnyTags(params Tag[] gameplayTags)
        {
            return gameplayTags.Any(HasTag);
        }

        public bool HasAnyPartialTag(params Tag[] gameplayTags)
        {
            return gameplayTags.Any(HasTag) || gameplayTags.Any(HasPartialTag);
        }
        
        public string DebugString()
        {
            var inherentTags = Tags.Aggregate("Inherent Tags\n", (current, tag) => current + (tag.Name + "\n"));
            var effectTags = "Effect Tags\n";
            foreach (var tag in EffectTags)
            {
                effectTags += tag.Key.Name + " (";
                effectTags = tag.Value.Aggregate(effectTags,
                    (current, effect) => current + (effect.Definition.name + " "));
                effectTags += ")\n";
            }
            var abilityTags = "Ability Tags\n";
            foreach (var tag in AbilityTags)
            {
                abilityTags += tag.Key.Name + " (";
                abilityTags = tag.Value.Aggregate(abilityTags,
                    (current, ability) => current + (ability + " "));
                abilityTags += ")\n";
            }
            return inherentTags + "\n" + effectTags + "\n" + abilityTags + "\n";
        }
    }
}