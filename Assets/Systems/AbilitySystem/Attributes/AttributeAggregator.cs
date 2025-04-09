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
        }

        public void Disable()
        {
            _asc.EffectSystem.UnregisterOnEffectAdded(RefreshModifierCache);
            _asc.EffectSystem.UnregisterOnEffectRemoved(RefreshModifierCache);
        }

        private float CalculateCurrentValue()
        {
            float newValue = _attribute.BaseValue;
            foreach (var tuple in _modifierCache)
            {
                var spec = tuple.Item1;
                var modifier = tuple.Item2;
                var magnitude = modifier.CalculateModifier(spec);

                switch (modifier.operation)
                {
                    case EffectOperation.Additive:
                        newValue += magnitude;
                        break;
                    case EffectOperation.Subtractive:
                        newValue -= magnitude;
                        break;
                    case EffectOperation.Multiplicative:
                        newValue *= magnitude;
                        break;
                    case EffectOperation.Divisive:
                        newValue /= magnitude;
                        break;
                    case EffectOperation.Override:
                        newValue = magnitude;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();   
                }
            }

            return newValue;
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
    }
}