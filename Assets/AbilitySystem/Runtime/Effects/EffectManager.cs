using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;

namespace AbilitySystem.Runtime.Effects
{
    public class EffectManager
    {
        private IAbilitySystem _owner;
        public List<Effect> Effects { get; private set; }
        public Dictionary<int, List<Effect>> PredictedEffects { get; private set; }

        public Action OnEffectAdded;
        public Action OnEffectRemoved;
        
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
            if (_owner.IsLocalClient()) return;
            _effectSnapshot.AddRange(Effects);
            _effectSnapshot.ForEach(e=>e.Tick());
            _effectSnapshot.Clear();
        }

        public List<Effect> GetActiveEffects()
        {
            List<Effect> activeEffects = Effects.Where(effect => effect.IsActive).ToList();
    
            foreach (List<Effect> effectList in PredictedEffects.Values)
            {
                activeEffects.AddRange(effectList.Where(effect => effect.IsActive));
            }

            return activeEffects;
        }

        public void AddEffect(Effect effect)
        {
            Effects.Add(effect);
            OnEffectAdded?.Invoke();
        }

        public void RemoveEffect(Effect effect)
        {
            Effects.Remove(effect);
            OnEffectRemoved?.Invoke();
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
            OnEffectAdded?.Invoke();
        }
        
        public void ReconcilePredictedEffect(PredictionKey predictionKey)
        {
            if (PredictedEffects.TryGetValue(predictionKey.currentKey, out var confirmedEffects))
            {
                Effects.AddRange(confirmedEffects);
                PredictedEffects.Remove(predictionKey.currentKey);
            }
        }

        public void RetractPredictedEffect(PredictionKey predictionKey)
        {
            PredictedEffects.Remove(predictionKey.currentKey);
            OnEffectRemoved?.Invoke();
        }
    }
}