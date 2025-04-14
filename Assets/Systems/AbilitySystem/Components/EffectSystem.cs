using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using Systems.AbilitySystem.Effects;

namespace Systems.AbilitySystem.Components
{
    public class EffectSystem
    {
        private AbilitySystemComponent _asc;

        private readonly Dictionary<int, EffectSpec> _effectSpecs = new();
        private readonly List<EffectSpec> _effectSpecsSnapshot = new();

        public Action<int, EffectSpec> OnEffectAdded;
        public Action<int> OnEffectRemoved;

        private int _key = 0;
        
        public EffectSystem(AbilitySystemComponent owner)
        {
            _asc = owner;
        }
        
        public void Initialise(AbilitySystemComponent owner)
        {
            _asc = owner;
        }
        
        public void Tick()
        {
            _effectSpecsSnapshot.AddRange(_effectSpecs.Values);
            foreach (var effectSpec in _effectSpecsSnapshot)
            {
                if (effectSpec.IsActive) effectSpec.Tick();
            }
            _effectSpecsSnapshot.Clear();
        }

        public List<EffectSpec> GetAllEffects()
        {
            return _effectSpecs.Values.ToList();
        }

        public EffectSpec AddEffectSpec(AbilitySystemComponent source, EffectSpec effectSpec)
        {
            if (effectSpec.DurationType == EffectDurationType.Instant)
            {
                effectSpec.Initialise(source, _asc, effectSpec.Level, -1);
                effectSpec.TriggerOnExecute();
                return null;
            }

            if (effectSpec.EffectStack.EffectStackType == EffectStackType.None)
            {
                return AddNewEffectSpec(source, NextKey(), effectSpec);
            }

            if (effectSpec.EffectStack.EffectStackType == EffectStackType.AggregateByTarget)
            {
                
            }
            
            if (effectSpec.EffectStack.EffectStackType == EffectStackType.AggregateBySource)
            {
                
            }

            return null;
        }

        public void AddEffectSpecClient(int key, EffectSpec effectSpec)
        {
            effectSpec.EffectApplicationKey = key;
            effectSpec.IsActive = true;
            _effectSpecs.Add(key, effectSpec);
        }
        
        public void RemoveEffect(int key)
        {
            _effectSpecs.Remove(key);
            OnEffectRemoved?.Invoke(key);
        }

        
        public void RegisterOnEffectAdded(Action<int, EffectSpec> action)
        {
            OnEffectAdded += action;
        }
        
        public void RegisterOnEffectRemoved(Action<int> action)
        {
            OnEffectRemoved += action;
        }

        public void UnregisterOnEffectAdded(Action<int, EffectSpec> action)
        {
            OnEffectAdded -= action;
        }
        
        public void UnregisterOnEffectRemoved(Action<int> action)
        {
            OnEffectRemoved -= action;
        }

        private EffectSpec AddNewEffectSpec(AbilitySystemComponent source, int key, EffectSpec effectSpec)
        {
            effectSpec.Initialise(source, _asc, effectSpec.Level, key);
            _effectSpecs.Add(key, effectSpec);
            // effectSpec.TriggerOnAdd();
            effectSpec.Activate();
            OnEffectAdded?.Invoke(key, effectSpec);
            return effectSpec;
        }

        private int NextKey()
        {
            _key++;
            return _key;
        }
    }
}