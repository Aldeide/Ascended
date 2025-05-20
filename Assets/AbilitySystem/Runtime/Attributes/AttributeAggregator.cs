using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Scripts;
using UnityEngine;

namespace AbilitySystem.Runtime.Attributes
{
    /// <summary>
    /// The AttributeAggregator class serves as a mechanism for aggregating and managing attribute modifiers.
    /// It is responsible for tracking and updating the values of specified attributes based on effects and modifiers
    /// applied to an entity within an ability system.
    /// </summary>
    public class AttributeAggregator
    {
        private readonly Attribute _attribute;
        private readonly IAbilitySystem _owner;
        
        private readonly List<Tuple<Effect, Modifier>> _modifierCache = new();

        public AttributeAggregator(Attribute attribute, IAbilitySystem owner)
        {
            _attribute = attribute;
            _owner = owner;
        }

        public List<Tuple<Effect, Modifier>> GetModifiers()
        {
            return _modifierCache;
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
            var newValue = _attribute.BaseValue;

            float additiveModifiers = 0;
            float multiplicativeModifiers = 1;
            float overrideModifiers = 0;
            var hasOverride = false;
            
            foreach (var (effect, modifier) in _modifierCache)
            {
                for (var i = 0; i < effect.NumStacks; i++)
                {
                    var magnitude = modifier.Calculate(effect);
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
            }
            //_owner.NotifyAttributeBaseChanged(_attribute.attributeSetName, _attribute.attributeName, newValue);
            return hasOverride ? overrideModifiers : (newValue + additiveModifiers) * multiplicativeModifiers;
        }
        
        private void RefreshModifierCache(Effect effect)
        {
            // TODO: only do this if the added or removed modifier concerns this attribute.
            _modifierCache.Clear();
            var effects = _owner.EffectManager.GetActiveEffects();
            foreach (var effectSpec in effects)
            {
                if (!effectSpec.IsActive) continue;
                if (effectSpec.Definition.modifiers == null) continue;
                foreach (var modifier in effectSpec.Definition.modifiers)
                {
                    if (modifier.attributeName == _attribute.GetFullName())
                    {
                        _modifierCache.Add(new Tuple<Effect, Modifier>(effectSpec, modifier));
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

            var newValue = CalculateCurrentValue();
            _attribute.SetCurrentValue(newValue);
        }
    }
}