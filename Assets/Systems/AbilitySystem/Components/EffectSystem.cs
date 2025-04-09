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

        public Action OnEffectAdded;
        public Action OnEffectRemoved;

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

        public void AddEffectSpec(AbilitySystemComponent source, EffectSpec effectSpec)
        {
            effectSpec.Initialise(source, _asc, 1);
            effectSpec.Activate();
            _effectSpecs.Add(effectSpec);
            OnEffectAdded.Invoke();
        }

        public void RemoveEffect(EffectSpec effectSpec)
        {
            _effectSpecs.Remove(effectSpec);
            OnEffectRemoved?.Invoke();
        }

        public void RegisterOnEffectAdded(Action action)
        {
            OnEffectAdded += action;
        }
        
        public void RegisterOnEffectRemoved(Action action)
        {
            OnEffectRemoved += action;
        }

        public void UnregisterOnEffectAdded(Action action)
        {
            OnEffectAdded -= action;
        }
        
        public void UnregisterOnEffectRemoved(Action action)
        {
            OnEffectRemoved -= action;
        }
    }
}