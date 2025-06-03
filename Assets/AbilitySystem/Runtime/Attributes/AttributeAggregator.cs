using System;
using System.Collections.Generic;
using System.Linq;
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
        private const float DivisionByZeroThreshold = float.Epsilon;

        private readonly Attribute _attribute;
        private readonly IAbilitySystem _owner;
        private readonly List<(Effect Effect, Modifier Modifier)> _modifierCache = new();


        /// <summary>
        /// Creates a new attribute aggregator for the specified attribute.
        /// </summary>
        /// <param name="attribute">The attribute to manage.</param>
        /// <param name="owner">The ability system that owns this attribute.</param>
        public AttributeAggregator(Attribute attribute, IAbilitySystem owner)
        {
            _attribute = attribute;
            _owner = owner;
        }

        /// <summary>
        /// Returns a read-only list of all modifiers currently affecting this attribute.
        /// </summary>
        public IReadOnlyList<(Effect Effect, Modifier Modifier)> GetModifiers()
        {
            return _modifierCache.AsReadOnly();
        }

        /// <summary>
        /// Enables the aggregator and starts tracking attribute modifications.
        /// </summary>
        public void Enable()
        {
            RegisterEventHandlers();
            InitializeAttributeState();
        }

        /// <summary>
        /// Disables the aggregator and stops tracking attribute modifications.
        /// </summary>
        public void Disable()
        {
            UnregisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            _owner.EffectManager.OnEffectAdded += ProcessEffectChange;
            _owner.EffectManager.OnEffectRemoved += ProcessEffectChange;
            _attribute.OnAttributeBaseValueChanged += HandleBaseValueChanged;
        }

        private void UnregisterEventHandlers()
        {
            _owner.EffectManager.OnEffectAdded -= ProcessEffectChange;
            _owner.EffectManager.OnEffectRemoved -= ProcessEffectChange;
            _attribute.OnAttributeBaseValueChanged -= HandleBaseValueChanged;
        }

        private void HandleBaseValueChanged(Attribute attribute, float oldBaseValue, float newBaseValue)
        {
            if (Mathf.Approximately(oldBaseValue, newBaseValue)) return;
            UpdateCurrentValue();
        }

        private void ProcessEffectChange(Effect changedEffect)
        {
            if (!IsEffectRelevantToAttribute(changedEffect)) return;
            BuildModifierCache();
            UpdateCurrentValue();
        }

        private void InitializeAttributeState()
        {
            BuildModifierCache();
            UpdateCurrentValue();
        }

        private void BuildModifierCache()
        {
            _modifierCache.Clear();
            var activeEffects = _owner.EffectManager.GetActiveEffects();
            foreach (var effect in activeEffects)
            {
                AddModifiersFromEffectToCache(effect);
            }
        }

        private void AddModifiersFromEffectToCache(Effect activeEffect)
        {
            if (!activeEffect.IsActive || activeEffect.Definition.modifiers == null) return;

            foreach (var modifier in activeEffect.Definition.modifiers)
            {
                if (modifier.attributeName == _attribute.GetFullName())
                {
                    _modifierCache.Add((activeEffect, modifier));
                }
            }
        }

        private bool IsEffectRelevantToAttribute(Effect effect)
        {
            return effect.Definition.modifiers != null &&
                   effect.Definition.modifiers.Any(mod => mod.attributeName == _attribute.GetFullName());
        }

        private void UpdateCurrentValue()
        {
            var newValue = CalculateCurrentValue();
            _attribute.SetCurrentValue(newValue);
        }

        private float CalculateCurrentValue()
        {
            var totals = CalculateModifierTotals();

            if (totals.HasOverride)
            {
                return totals.OverrideValue;
            }

            return (_attribute.BaseValue + totals.AdditiveValue) * totals.MultiplicativeValue;
        }

        private ModifierTotals CalculateModifierTotals()
        {
            var totals = ModifierTotals.Create();

            foreach (var (effect, modifier) in _modifierCache)
            {
                for (var i = 0; i < effect.NumStacks; i++)
                {
                    var magnitude = modifier.Calculate(effect);
                    ApplyModifierToTotals(modifier.operation, magnitude, ref totals);
                }
            }

            return totals;
        }

        private static void ApplyModifierToTotals(EffectOperation operation, float magnitude, ref ModifierTotals totals)
        {
            switch (operation)
            {
                case EffectOperation.Additive:
                    totals.AdditiveValue += magnitude;
                    break;
                case EffectOperation.Subtractive:
                    totals.AdditiveValue -= magnitude;
                    break;
                case EffectOperation.Multiplicative:
                    totals.MultiplicativeValue *= magnitude;
                    break;
                case EffectOperation.Divisive:
                    if (Math.Abs(magnitude) < DivisionByZeroThreshold)
                    {
                        Debug.LogWarning("Divisive modifier has a magnitude of zero. Skipping division.");
                        break;
                    }
                    totals.MultiplicativeValue /= magnitude;
                    break;
                case EffectOperation.Override:
                    totals.HasOverride = true;
                    totals.OverrideValue = magnitude;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation,
                        "Unsupported effect operation.");
            }
        }


        private struct ModifierTotals
        {
            // Fields
            public float AdditiveValue;
            public float MultiplicativeValue;
            public float OverrideValue;
            public bool HasOverride;

            // Initialize with a static method instead of a constructor
            public static ModifierTotals Create()
            {
                return new ModifierTotals
                {
                    AdditiveValue = 0,
                    MultiplicativeValue = 1,
                    OverrideValue = 0,
                    HasOverride = false
                };
            }
        }
    }
}