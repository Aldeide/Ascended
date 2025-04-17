using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Scripts;
using UnityEngine;

namespace AbilitySystem.Runtime.Attributes
{
    public class AttributeAggregator
    {
        private readonly Attribute _attribute;
        private readonly IAbilitySystem _owner;
        
        private List<Tuple<Effect, EffectModifier>> _modifierCache = new();

        public AttributeAggregator(Attribute attribute, IAbilitySystem owner)
        {
            _attribute = attribute;
            _owner = owner;
        }

        public void Enable()
        {
            _owner.EffectManager.OnEffectAdded += RefreshModifierCache;
            _owner.EffectManager.OnEffectRemoved += RefreshModifierCache;
            _attribute.OnAttributeBaseValueChanged += UpdateCurrentValueWhenBaseValueChanged;
        }

        public void Disable()
        {
            _owner.EffectManager.OnEffectAdded -= RefreshModifierCache;
            _owner.EffectManager.OnEffectRemoved -= RefreshModifierCache;
            _attribute.OnAttributeBaseValueChanged -= UpdateCurrentValueWhenBaseValueChanged;
        }

        private float CalculateCurrentValue()
        {
            float newValue = _attribute.BaseValue;

            float additiveModifiers = 0;
            float multiplicativeModifiers = 1;
            float overrideModifiers = 0;
            bool hasOverride = false;
            
            foreach (var tuple in _modifierCache)
            {
                var spec = tuple.Item1;
                var modifier = tuple.Item2;
                var magnitude = modifier.CalculateModifier(spec);

                switch (modifier.operation)
                {
                    case EffectOperation.Additive:
                        additiveModifiers += magnitude;
                        break;
                    case EffectOperation.Subtractive:
                        additiveModifiers -= magnitude;
                        break;
                    case EffectOperation.Multiplicative:
                        multiplicativeModifiers *= magnitude;
                        break;
                    case EffectOperation.Divisive:
                        multiplicativeModifiers /= magnitude;
                        break;
                    case EffectOperation.Override:
                        // TODO: currently only the latest override is considered. Might want to define a property for 
                        // attributes as to whether they prefer the smallest or largest override.
                        hasOverride = true;
                        overrideModifiers = magnitude;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();   
                }
            }
            //_owner.NotifyAttributeBaseChanged(_attribute.attributeSetName, _attribute.attributeName, newValue);
            return hasOverride ? overrideModifiers : (newValue + additiveModifiers) * multiplicativeModifiers;
        }

        private void OnEffectRemoved()
        {
            RefreshModifierCache();
        }
        
        private void RefreshModifierCache()
        {
            // TODO: only do this if the added or removed modifier concerns this attribute.
            _modifierCache.Clear();
            var effects = _owner.EffectManager.GetActiveEffects();
            foreach (var effectSpec in effects)
            {
                if (effectSpec.IsActive)
                {
                    foreach (var modifier in effectSpec.Definition.Asset.Modifiers)
                    {
                        if (modifier.attributeName == _attribute.GetName())
                        {
                            _modifierCache.Add(new Tuple<Effect, EffectModifier>(effectSpec, modifier));
                        }
                    }
                }
            }
            UpdateCurrentValue();
        }

        private void UpdateCurrentValue()
        {
            float newValue = CalculateCurrentValue();
            _attribute.SetCurrentValue(newValue);
            //_owner.NotifyAttributeCurrentChanged(_attribute.a, _attribute.attributeName, newValue);
        }

        private void UpdateCurrentValueWhenBaseValueChanged(Attribute attribute, float oldBaseValue, float newBaseValue)
        {
            if (Mathf.Approximately(oldBaseValue, newBaseValue)) return;

            float newValue = CalculateCurrentValue();
            _attribute.SetCurrentValue(newValue);
        }
    }
}