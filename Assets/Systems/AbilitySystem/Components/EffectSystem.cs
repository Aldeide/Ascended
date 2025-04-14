using System;
using System.Collections.Generic;
using FishNet.Object;
using Systems.AbilitySystem.Effects;

namespace Systems.AbilitySystem.Components
{
    public class EffectSystem
    {
        private AbilitySystemComponent _asc;

        private readonly List<EffectSpec> _effectSpecs = new List<EffectSpec>();
        private readonly List<EffectSpec> _effectSpecsSnapshot = new List<EffectSpec>();

        public Action<EffectSpec> OnEffectAdded;
        public Action<EffectSpec> OnEffectRemoved;

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
            _effectSpecsSnapshot.AddRange(_effectSpecs);
            foreach (var effectSpec in _effectSpecsSnapshot)
            {
                if (effectSpec.IsActive) effectSpec.Tick();
            }
            _effectSpecsSnapshot.Clear();
        }

        public List<EffectSpec> GetAllEffects()
        {
            return _effectSpecs;
        }

        public EffectSpec AddEffectSpec(AbilitySystemComponent source, EffectSpec effectSpec)
        {
            if (effectSpec.DurationType == EffectDurationType.Instant)
            {
                effectSpec.Initialise(source, _asc, effectSpec.Level);
                effectSpec.TriggerOnExecute();
                return null;
            }

            if (effectSpec.EffectStack.EffectStackType == EffectStackType.None)
            {
                return AddNewEffectSpec(source, effectSpec);
            }

            if (effectSpec.EffectStack.EffectStackType == EffectStackType.AggregateByTarget)
            {
                
            }
            
            if (effectSpec.EffectStack.EffectStackType == EffectStackType.AggregateBySource)
            {
                
            }

            return null;
        }

        public void AddEffectSpecClient(EffectSpec effectSpec)
        {
            _effectSpecs.Add(effectSpec);
        }
        
        public void RemoveEffect(EffectSpec effectSpec)
        {
            _effectSpecs.Remove(effectSpec);
            OnEffectRemoved?.Invoke(effectSpec);
        }

        
        public void RegisterOnEffectAdded(Action<EffectSpec> action)
        {
            OnEffectAdded += action;
        }
        
        public void RegisterOnEffectRemoved(Action<EffectSpec> action)
        {
            OnEffectRemoved += action;
        }

        public void UnregisterOnEffectAdded(Action<EffectSpec> action)
        {
            OnEffectAdded -= action;
        }
        
        public void UnregisterOnEffectRemoved(Action<EffectSpec> action)
        {
            OnEffectRemoved -= action;
        }

        private EffectSpec AddNewEffectSpec(AbilitySystemComponent source, EffectSpec effectSpec)
        {
            effectSpec.Initialise(source, _asc, effectSpec.Level);
            _effectSpecs.Add(effectSpec);
            // effectSpec.TriggerOnAdd();
            effectSpec.Activate();
            OnEffectAdded?.Invoke(effectSpec);
            return effectSpec;
        }
    }
}