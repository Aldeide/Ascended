using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using JetBrains.Annotations;
using UnityEngine;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace AbilitySystem.Runtime.AttributeSets
{
    /// <summary>
    /// Manages the lifecycle and operations of attribute sets and their values within an ability system.
    /// Provides functionality to add, retrieve, and manipulate attribute sets and their associated attributes.
    /// </summary>
    public class AttributeSetManager : IDisposable
    {
        public Action<Attribute, float, float> OnAnyAttributeBaseValueChanged;
        public Action<Attribute, float, float> OnAnyAttributeCurrentValueChanged;

        private IAbilitySystem _owner;
        public Dictionary<Type, AttributeSet> AttributeSets;
        private Dictionary<string, AttributeAggregator> _attributeAggregators;
        private Dictionary<string, Attribute> _attributeFullNameCache;
        private List<Attribute> _allAttributesList = new List<Attribute>();
        public Attribute CurrentlyCalculatingAttribute;

        // Job related data
        private NativeArray<AttributeState> _attributeStates;
        private NativeArray<ModifierData> _allModifiers;
        private NativeArray<int2> _modifierRanges;
        private bool _isDirty = true;

        public AttributeSetManager(IAbilitySystem owner)
        {
            _owner = owner;
            AttributeSets = new Dictionary<Type, AttributeSet>();
            _attributeAggregators = new Dictionary<string, AttributeAggregator>();
            _attributeFullNameCache = new Dictionary<string, Attribute>();
        }

        public void Reset()
        {
            foreach (var attributeSet in AttributeSets.Values)
            {
                attributeSet.Reset();
            }
            _isDirty = true;
        }

        public void Dispose()
        {
            if (_attributeStates.IsCreated) _attributeStates.Dispose();
            if (_modifierRanges.IsCreated) _modifierRanges.Dispose();
            if (_allModifiers.IsCreated) _allModifiers.Dispose();
        }

        public virtual T GetAttributeSet<T>() where T : AttributeSet
        {
            AttributeSets.TryGetValue(typeof(T), out AttributeSet result);
            return (T)result;
        }

        [CanBeNull]
        public virtual AttributeSet GetAttributeSet(string attributeSetName)
        {
            return AttributeSets.Values.FirstOrDefault(a => a.Name == attributeSetName);
        }

        public void AddAttributeSet(Type type, AttributeSet attributeSet)
        {
            attributeSet.InitializeAttributes();
            AttributeSets[type] = attributeSet;
            foreach (var attribute in attributeSet.GetAllAttributes())
            {
                attribute.OnAttributeBaseValueChanged += OnAttributeBaseValueChanged;
                attribute.OnAttributeCurrentValueChanged += OnAttributeCurrentValueChanged;
                
                int index = _allAttributesList.Count;
                attribute.SetManager(this, index);
                _allAttributesList.Add(attribute);

                var aggregator = new AttributeAggregator(attribute, _owner);
                aggregator.Enable();
                _attributeAggregators.Add(attribute.GetName(), aggregator);
                _attributeFullNameCache.Add(attribute.GetFullName(), attribute);
            }
            ReallocateNativeData();
            _isDirty = true;
        }

        private void ReallocateNativeData()
        {
            if (_attributeStates.IsCreated) _attributeStates.Dispose();
            if (_modifierRanges.IsCreated) _modifierRanges.Dispose();

            _attributeStates = new NativeArray<AttributeState>(_allAttributesList.Count, Allocator.Persistent);
            _modifierRanges = new NativeArray<int2>(_allAttributesList.Count, Allocator.Persistent);

            for (int i = 0; i < _allAttributesList.Count; i++)
            {
                var attr = _allAttributesList[i];
                var val = attr.GetInternalValue();
                _attributeStates[i] = new AttributeState(val.BaseValue, val.MinValue, val.MaxValue);
            }
        }

        [CanBeNull]
        public virtual Attribute GetAttribute(string attributeName)
        {
            return AttributeSets.Values.SelectMany(attributeSet =>
                    attributeSet.GetAllAttributes().Where(attribute => attribute.GetName() == attributeName))
                .FirstOrDefault();
        }

        [CanBeNull]
        public virtual Attribute GetAttribute<T>(string attributeName)
        {
            return GetAttribute(typeof(T), attributeName);
        }

        [CanBeNull]
        public virtual Attribute GetAttribute(Type attributeSetType, string attributeName)
        {
            AttributeSets.TryGetValue(attributeSetType, out AttributeSet result);
            return result.GetAttribute(attributeName);
        }

        public virtual Attribute GetAttribute(string attributeSetName, string attributeName)
        {
            return AttributeSets.FirstOrDefault(
                k => k.Value.Name == attributeSetName).Value?.GetAttribute(attributeName);
        }

        public AttributeValue GetAttributeValue<T>(string attributeName) where T : AttributeSet
        {
            return GetAttribute<T>(attributeName).GetValue();
        }

        public Dictionary<string, AttributeValue> Snapshot()
        {
            Dictionary<string, AttributeValue> output = new Dictionary<string, AttributeValue>();
            foreach (var attributeSet in AttributeSets.Values)
            {
                foreach (var attribute in attributeSet.GetAllAttributes())
                {
                    output.Add(attribute.GetName(), attribute.GetValue());
                }
            }

            return output;
        }

        public void Restore(Dictionary<string, AttributeValue> snapshot)
        {
            foreach (var attributeSet in AttributeSets.Values)
            {
                foreach (var attribute in attributeSet.GetAllAttributes())
                {
                    if (snapshot.TryGetValue(attribute.GetName(), out var value))
                    {
                        attribute.Restore(value);
                    }
                }
            }
        }

        public void RegisterOnAttributeChanged(string attributeName, Action<Attribute, float, float> action)
        {
            foreach (var attributeSet in AttributeSets.Values)
            {
                foreach (var attribute in attributeSet.GetAllAttributes())
                {
                    if (attribute.GetName() == attributeName)
                    {
                        attribute.OnAttributeCurrentValueChanged += action;
                        attribute.OnAttributeBaseValueChanged += action;
                    }
                }
            }
        }

        public void ApplyInstantEffectModifiers(Effect instantEffect)
        {
            if (instantEffect.Definition.Modifiers != null)
            {
                foreach (var modifier in instantEffect.Definition.Modifiers)
                {
                    if (!_attributeFullNameCache.TryGetValue(modifier.AttributeName, out var attribute)) continue;
                    
                    var magnitude = modifier.Calculate(instantEffect);
                    
                    // Trigger PreGameplayEffectExecute callback. Can modify magnitude or cancel execution (by returning false).
                    if (attribute.AttributeSet != null)
                    {
                        if (!attribute.AttributeSet.PreGameplayEffectExecute(instantEffect, modifier, ref magnitude))
                        {
                            continue;
                        }
                    }
                    
                    var baseValue = attribute.BaseValue;
                    switch (modifier.Operation)
                    {
                        case EffectOperation.Additive:
                            baseValue += magnitude;
                            break;
                        case EffectOperation.Subtractive:
                            baseValue -= magnitude;
                            break;
                        case EffectOperation.Multiplicative:
                            baseValue *= magnitude;
                            break;
                        case EffectOperation.Divisive:
                            baseValue /= magnitude;
                            break;
                        case EffectOperation.Override:
                            baseValue = magnitude;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    attribute.SetBaseValue(baseValue);
                    
                    // Trigger PostGameplayEffectExecute callback
                    attribute.AttributeSet?.PostGameplayEffectExecute(instantEffect, modifier, magnitude);
                }
            }

            if (instantEffect.Definition.Executions != null)
            {
                foreach (var execution in instantEffect.Definition.Executions)
                {
                    execution?.Execute(instantEffect);
                }
            }
        }

        public void OnAttributeBaseValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            OnAnyAttributeBaseValueChanged?.Invoke(attribute, oldValue, newValue);
        }
        
        public void OnAttributeCurrentValueChanged(Attribute attribute, float oldValue, float newValue)
        {
            OnAnyAttributeCurrentValueChanged?.Invoke(attribute, oldValue, newValue);
        }

        public string DebugString()
        {
            return AttributeSets.Values.Aggregate(
                "Attributes\n", (current, attributeSet) => current + (attributeSet.DebugString() + "\n"));
        }

        public AttributeAggregator GetAggregator(string attributeName)
        {
            return _attributeAggregators[attributeName];
        }

        public void MarkDirty() => _isDirty = true;

        public void UpdateAttributesJobified()
        {
            if (!_isDirty) return;
            _isDirty = false;
            
            // 1. Collect all modifiers from aggregators and capture old values
            List<ModifierData> modsList = new List<ModifierData>();
            float[] oldValues = new float[_allAttributesList.Count];
            for (int i = 0; i < _allAttributesList.Count; i++)
            {
                oldValues[i] = _allAttributesList[i].CurrentValue;
                var attr = _allAttributesList[i];
                CurrentlyCalculatingAttribute = attr;
                var aggregator = _attributeAggregators[attr.GetName()];
                var mods = aggregator.GetModifiers();
                
                int start = modsList.Count;
                foreach (var mod in mods)
                {
                    // For now, we only handle static magnitudes. 
                    // Dynamic modifiers should be calculated on CPU and passed as static here.
                    for (int stack = 0; stack < mod.Effect.NumStacks; stack++)
                    {
                        modsList.Add(new ModifierData(mod.Modifier.Calculate(mod.Effect), mod.Modifier.Operation));
                    }
                }
                CurrentlyCalculatingAttribute = null;
                _modifierRanges[i] = new int2(start, modsList.Count - start);
                
                // Update base values in case they changed via SetBaseValue
                var state = _attributeStates[i];
                state.BaseValue = attr.BaseValue;
                state.MinValue = attr.GetValue().MinValue;
                state.MaxValue = attr.GetValue().MaxValue;
                _attributeStates[i] = state;
            }

            if (_allModifiers.IsCreated) _allModifiers.Dispose();
            _allModifiers = new NativeArray<ModifierData>(modsList.Count, Allocator.Persistent);
            _allModifiers.CopyFrom(modsList.ToArray());

            // 2. Schedule and Complete Job
            var job = new AttributeRecalculationJob
            {
                States = _attributeStates,
                AllModifiers = _allModifiers,
                ModifierRanges = _modifierRanges
            };

            JobHandle handle = job.Schedule(_allAttributesList.Count, 64);
            handle.Complete();

            // 3. Compare and fire events
            for (int i = 0; i < _allAttributesList.Count; i++)
            {
                var attr = _allAttributesList[i];
                var newValue = _attributeStates[i].CurrentValue;
                if (!Mathf.Approximately(oldValues[i], newValue))
                {
                    attr.SetCurrentValueNoEvent(newValue);
                    attr.OnAttributeCurrentValueChanged?.Invoke(attr, oldValues[i], newValue);
                    OnAnyAttributeCurrentValueChanged?.Invoke(attr, oldValues[i], newValue);
                    attr.AttributeSet?.PostAttributeChange(attr, oldValues[i], newValue);
                }
            }

        }

        public float GetAttributeBaseValue(int index) => _attributeStates[index].BaseValue;
        public float GetAttributeCurrentValue(int index)
        {
            if (_isDirty) UpdateAttributesJobified();
            return _attributeStates[index].CurrentValue;
        }

        public float GetAttributeMinValue(int index) => _attributeStates[index].MinValue;
        public float GetAttributeMaxValue(int index) => _attributeStates[index].MaxValue;

        public void SetAttributeBaseValue(int index, float value)
        {
            var state = _attributeStates[index];
            state.BaseValue = value;
            // Clamping
            state.BaseValue = math.clamp(state.BaseValue, state.MinValue, state.MaxValue);
            state.CurrentValue = state.BaseValue;
            _attributeStates[index] = state;
            _isDirty = true;
        }

        public void SetAttributeCurrentValue(int index, float value)
        {
            var state = _attributeStates[index];
            state.CurrentValue = value;
            // Clamping
            state.CurrentValue = math.clamp(state.CurrentValue, state.MinValue, state.MaxValue);
            _attributeStates[index] = state;
        }

        public void SetAttributeBaseValueNoEvent(int index, float value)
        {
            var state = _attributeStates[index];
            state.BaseValue = value;
            state.BaseValue = math.clamp(state.BaseValue, state.MinValue, state.MaxValue);
            state.CurrentValue = state.BaseValue;
            _attributeStates[index] = state;
        }

        public void SetAttributeCurrentValueNoEvent(int index, float value)
        {
            var state = _attributeStates[index];
            state.CurrentValue = value;
            state.CurrentValue = math.clamp(state.CurrentValue, state.MinValue, state.MaxValue);
            _attributeStates[index] = state;
        }
    }
}