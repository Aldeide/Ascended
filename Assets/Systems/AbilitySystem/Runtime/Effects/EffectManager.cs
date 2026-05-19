using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystem.Runtime.Effects
{
    public class EffectManager
    {
        private readonly IAbilitySystem _owner;
        public List<Effect> Effects { get; private set; }
        public Dictionary<int, List<Effect>> PredictedEffects { get; private set; }

        public Action<Effect> OnEffectAdded;
        public Action<Effect> OnEffectRemoved;
        public Action<Effect> OnEffectSuspended;
        public Action<Effect> OnEffectResumed;
        public Action<Effect> OnEffectRetracted;
        public Action<Effect, int, int> OnEffectStacksChanged;
        
        private readonly List<Effect> _effectSnapshot;
        
        public EffectManager(IAbilitySystem owner)
        {
            _owner = owner;
            Effects = new List<Effect>();
            PredictedEffects = new Dictionary<int, List<Effect>>();
            _effectSnapshot = new List<Effect>();
        }

        public void Tick()
        {
            // Only the server ticks effects.
            if (!_owner.IsServer()) return;
            _effectSnapshot.AddRange(Effects);
            _effectSnapshot.ForEach(e=>e.Tick());
            _effectSnapshot.Clear();
        }

        public List<Effect> GetActiveEffects()
        {
            var activeEffects = Effects.Where(effect => effect.IsActive).ToList();
            foreach (var effectList in PredictedEffects.Values)
            {
                activeEffects.AddRange(effectList.Where(effect => effect.IsActive));
            }

            return activeEffects;
        }

        public EffectApplicationResult AddEffect(Effect effect)
        {
            if (effect.Definition.ApplicationImmunityTags != null && _owner.TagManager.HasAnyTags(effect.Definition.ApplicationImmunityTags))
            {
                return EffectApplicationResult.Immune;
            }

            if (effect.Definition.ApplicationRequiredTags != null && !_owner.TagManager.HasAllTags(effect.Definition.ApplicationRequiredTags))
            {
                return EffectApplicationResult.ApplicationRequiredTagsFailure;
            }

            if (effect.Definition.ApplicationRequirements != null)
            {
                foreach (var req in effect.Definition.ApplicationRequirements)
                {
                    if (req != null && !req.CanApplyEffect(_owner, effect.Source, effect))
                    {
                        return EffectApplicationResult.FailedCustomRequirement;
                    }
                }
            }

            if (effect.Definition.RemoveGameplayEffectsWithTags != null && effect.Definition.RemoveGameplayEffectsWithTags.Length > 0)
            {
                var effectsToRemove = Effects.Where(e => 
                    (e.Definition.AssetTags != null && e.Definition.AssetTags.Intersect(effect.Definition.RemoveGameplayEffectsWithTags).Any()) ||
                    (e.Definition.GrantedTags != null && e.Definition.GrantedTags.Intersect(effect.Definition.RemoveGameplayEffectsWithTags).Any())
                ).ToList();

                foreach (var e in effectsToRemove)
                {
                    RemoveEffect(e);
                }
            }

            if (effect.Definition.IsInstant())
            {
                effect.Execute();
                if (_owner.IsServer())
                {
                    _owner.ReplicationManager.NotifyClientsEffectAdded(effect);
                }
                return EffectApplicationResult.Success;
            }

            // If that effect is already applied, check stacking behaviour.
            var existingEffect = Effects.FirstOrDefault(e => e.Definition.name == effect.Definition.name);
            if (existingEffect != null)
            {
                // SPECIAL CASE: If we are on a client and we receive a REPLICATED effect 
                // that matches an existing NON-REPLICATED effect, we replace it.
                // This handles predicted effects (like cooldowns) that don't use formal prediction keys.
                if (!_owner.IsServer() && effect.IsReplicated && !existingEffect.IsReplicated)
                {
                    RemoveEffect(existingEffect);
                    Effects.Add(effect);
                    OnEffectAdded?.Invoke(effect);
                    return EffectApplicationResult.Success;
                }

                if (existingEffect.Definition.EffectStack.EffectStackType == EffectStackType.None)
                {
                    Effects.Add(effect);
                    OnEffectAdded?.Invoke(effect);
                    if (_owner.IsServer())
                    {
                        _owner.ReplicationManager.NotifyClientsEffectAdded(effect);
                    }
                    return EffectApplicationResult.Success;
                }

                if (existingEffect.NumStacks >= existingEffect.Definition.EffectStack.MaxStacks)
                {
                    if (existingEffect.Definition.EffectStack.EffectStackOverflowPolicy.DenyOverflowApplication)
                    {
                        return EffectApplicationResult.OverflowDeny;
                    }

                    if (existingEffect.Definition.EffectStack.EffectStackOverflowPolicy.ClearStackOnOverflow)
                    {
                        RemoveEffect(existingEffect);
                        return EffectApplicationResult.OverflowClear;
                    }
                }

                if (existingEffect.Definition.EffectStack.EffectStackType == EffectStackType.AggregateByTarget)
                {
                    if (!_owner.IsServer() && effect.IsReplicated && existingEffect.IsReplicated)
                    {
                        var oldStacks = existingEffect.NumStacks;
                        existingEffect.NumStacks = effect.NumStacks;
                        if (existingEffect.Definition.EffectStack.EffectStackDurationPolicy == EffectStackDurationPolicy.RefreshOnNewApplication)
                        {
                            existingEffect.RefreshDuration();
                        }
                        if (oldStacks != existingEffect.NumStacks)
                        {
                            OnEffectStacksChanged?.Invoke(existingEffect, oldStacks, existingEffect.NumStacks);
                        }
                        return EffectApplicationResult.Success;
                    }

                    existingEffect.AddStack();
                    return EffectApplicationResult.Success;
                }
                
                if (existingEffect.Definition.EffectStack.EffectStackType == EffectStackType.AggregateBySource)
                {
                    var existingEffectFromSource = Effects.FirstOrDefault(e => e.Definition.name == effect.Definition.name && e.Source == effect.Source);
                    if (existingEffectFromSource != null)
                    {
                        if (!_owner.IsServer() && effect.IsReplicated && existingEffectFromSource.IsReplicated)
                        {
                            var oldStacks = existingEffectFromSource.NumStacks;
                            existingEffectFromSource.NumStacks = effect.NumStacks;
                            if (existingEffectFromSource.Definition.EffectStack.EffectStackDurationPolicy == EffectStackDurationPolicy.RefreshOnNewApplication)
                            {
                                existingEffectFromSource.RefreshDuration();
                            }
                            if (oldStacks != existingEffectFromSource.NumStacks)
                            {
                                OnEffectStacksChanged?.Invoke(existingEffectFromSource, oldStacks, existingEffectFromSource.NumStacks);
                            }
                            return EffectApplicationResult.Success;
                        }

                        existingEffectFromSource.AddStack();
                        return EffectApplicationResult.Success;
                    }
                }
            }
            Effects.Add(effect);
            OnEffectAdded?.Invoke(effect);
            if (_owner.IsServer())
            {
                _owner.ReplicationManager.NotifyClientsEffectAdded(effect);
            }
            return EffectApplicationResult.Success;
        }

        public void RemoveEffect(Effect effect)
        {
            Effects.Remove(effect);
            OnEffectRemoved?.Invoke(effect);
            if (_owner.IsServer())
            {
                _owner.ReplicationManager.NotifyClientsEffectRemoved(effect);
            }
        }

        public void RemoveEffect(string effectName)
        {
            var effectToRemove = Effects.Where(e => e.Definition.name == effectName).OrderBy(e => e.ActivationTime).FirstOrDefault();
            if (effectToRemove != null)
            {
                RemoveEffect(effectToRemove);
            }
        }

        public void AddEffectFromServer(Effect effect)
        {
            effect.IsReplicated = true;
            effect.IsActive = true;
            AddEffect(effect);
        }

        public void AddPredictedEffect(PredictionKey predictionKey, Effect predictedEffect)
        {
            if (PredictedEffects.TryGetValue(predictionKey.currentKey, out var existingEffects))
            {
                existingEffects.Add(predictedEffect);
            }
            else
            {
                PredictedEffects[predictionKey.currentKey] = new List<Effect> { predictedEffect };
            }
            if (predictedEffect.Definition.IsInstant())
            {
                predictedEffect.Execute();
            }
            OnEffectAdded?.Invoke(predictedEffect);
        }
        
        public void ReconcilePredictedEffect(PredictionKey predictionKey, Effect serverEffect)
        {
            RetractPredictedEffect(predictionKey);
            AddEffectFromServer(serverEffect);
        }

        public void RetractPredictedEffect(PredictionKey predictionKey)
        {
            if (PredictedEffects.TryGetValue(predictionKey.currentKey, out var retractedEffects))
            {
                foreach (var effect in retractedEffects)
                {
                    effect.IsActive = false;
                    Effects.Remove(effect);
                    OnEffectRetracted?.Invoke(effect);
                    OnEffectRemoved?.Invoke(effect);
                }
                PredictedEffects.Remove(predictionKey.currentKey);
            }
        }

        public Effect GetEffect(Tag assetTag)
        {
            return Effects.FirstOrDefault(e=>e.Definition.AssetTags.Contains(assetTag));
        }

        public string DebugString()
        {
            var output = Effects.Aggregate(
                "Effects\n", (current, effect) => current + (effect.DebugString() + "\n"));
            foreach (var effect in PredictedEffects)
            {
                output += "Prediction Key " + effect.Key;
                output = effect.Value.Aggregate(output, (current, e) => current + (e.DebugString() + "\n"));
            }
            return output;
        }
    }
}