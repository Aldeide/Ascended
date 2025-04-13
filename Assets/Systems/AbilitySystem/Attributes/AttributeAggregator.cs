using System;
using System.Collections.Generic;
using Systems.AbilitySystem.Components;
using Systems.AbilitySystem.Effects;
using Systems.AbilitySystem.Effects.Modifiers;
using Unity.VisualScripting;
using UnityEngine;

namespace Systems.AbilitySystem.Attributes
{
    public class AttributeAggregator
    {
        private readonly AttributeBase _attribute;
        private readonly AbilitySystemComponent _asc;
        
        private List<Tuple<EffectSpec, EffectModifier>> _modifierCache = new();

        public AttributeAggregator(AttributeBase attribute, AbilitySystemComponent owner)
        {
            _attribute = attribute;
            _asc = owner;
        }

        public void Enable()
        {
            _asc.EffectSystem.RegisterOnEffectAdded(RefreshModifierCache);
            _asc.EffectSystem.RegisterOnEffectRemoved(RefreshModifierCache);
            _attribute.RegisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueChanged);
        }

        public void Disable()
        {
            _asc.EffectSystem.UnregisterOnEffectAdded(RefreshModifierCache);
            _asc.EffectSystem.UnregisterOnEffectRemoved(RefreshModifierCache);
            _attribute.UnregisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueChanged);
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
            _asc.NotifyAttributeBaseChanged(_attribute.attributeSetName, _attribute.attributeName, newValue);
            return hasOverride ? overrideModifiers : (newValue + additiveModifiers) * multiplicativeModifiers;
        }

        private void RefreshModifierCache()
        {
            // TODO: only do this if the added or removed modifier concerns this attribute.
            _modifierCache.Clear();
            var effects = _asc.EffectSystem.GetAllEffects();
            foreach (var effectSpec in effects)
            {
                if (effectSpec.IsActive)
                {
                    foreach (var modifier in effectSpec.Modifiers)
                    {
                        if (modifier.attributeName == _attribute.AttributeName())
                        {
                            _modifierCache.Add(new Tuple<EffectSpec, EffectModifier>(effectSpec, modifier));
                        }
                    }
                }
            }
            UpdateCurrentValue();
        }

        private void UpdateCurrentValue()
        {
            _attribute.SetCurrentValue(CalculateCurrentValue());
        }

        private void UpdateCurrentValueWhenBaseValueChanged(AttributeBase attribute, float oldBaseValue, float newBaseValue)
        {
            if (Mathf.Approximately(oldBaseValue, newBaseValue)) return;

            float newValue = CalculateCurrentValue();
            _attribute.SetCurrentValue(newValue);
        }
    }
}