using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
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
            // If that effect is already applied, check stacking behaviour.
            if (Effects.Exists(e => e.Definition.name == effect.Definition.name))
            {
                var existingEffect = Effects.FirstOrDefault(e => e.Definition.name == effect.Definition.name);
                if (existingEffect.Definition.EffectStack.EffectStackType == EffectStackType.None)
                {
                    Effects.Add(effect);
                    OnEffectAdded?.Invoke(effect);
                    return EffectApplicationResult.Success;
                }

                if (existingEffect.Definition.EffectStack.EffectStackType == EffectStackType.AggregateByTarget)
                {
                    existingEffect.AddStack();
                    OnEffectAdded?.Invoke(effect);
                    return EffectApplicationResult.Success;
                }
                // TODO: aggregate by source.
            }
            Effects.Add(effect);
            OnEffectAdded?.Invoke(effect);
            return EffectApplicationResult.Success;
        }

        public void RemoveEffect(Effect effect)
        {
            Effects.Remove(effect);
            OnEffectRemoved?.Invoke(effect);
        }

        public void RemoveEffect(string effectName)
        {
            var effectToRemove = Effects.FirstOrDefault(e=>e.Definition.name == effectName);
            if (effectToRemove != null)
            {
                RemoveEffect(effectToRemove);
            }
        }

        public void AddEffectFromServer(Effect effect)
        {
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
            OnEffectAdded?.Invoke(predictedEffect);
        }
        
        public void ReconcilePredictedEffect(PredictionKey predictionKey)
        {
            if (!PredictedEffects.TryGetValue(predictionKey.currentKey, out var confirmedEffects)) return;
            Effects.AddRange(confirmedEffects);
            PredictedEffects.Remove(predictionKey.currentKey);
        }

        public void RetractPredictedEffect(PredictionKey predictionKey)
        {
            PredictedEffects.Remove(predictionKey.currentKey);
        }

        public Effect GetEffect(GameplayTag assetTag)
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