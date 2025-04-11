using System;
using System.Collections.Generic;
using FishNet.Object;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.AbilitySystem.Attributes;
using Systems.AbilitySystem.Effects;
using Systems.Attributes;
using UnityEngine;

namespace Systems.AbilitySystem.Components
{
    [Serializable]
    public class AttributeSystem
    {
        
        [ShowInInspector]
        private Dictionary<string, AttributeSet> _attributeSets = new();
        [ShowInInspector]
        private readonly Dictionary<AttributeBase, AttributeAggregator> _attributeAggregators = new();
        
        private AbilitySystemComponent _asc;
        private readonly List<EffectSpec> _effectSpecsSnapshot = new List<EffectSpec>();

        public AttributeSystem(AbilitySystemComponent owner)
        {
            _asc = owner;
        }
        
        public void Initialise(AbilitySystemComponent owner)
        {
            _asc = owner;
            _asc.EffectSystem.OnEffectAdded += OnEffectAdded;
        }

        public List<AttributeBase> GetAllAttributes()
        {
            List<AttributeBase> attributes = new List<AttributeBase>();
            foreach (var attributeSet in _attributeSets.Values)
            {
                foreach (var attribute in attributeSet.Attributes.Values)
                {
                    attributes.Add(attribute);
                }
            }

            return attributes;
        }

        public void AddAttributeSet<T>() where T : AttributeSet
        {
            AddAttributeSet(typeof(T));
        }
        
        public void AddAttributeSet(Type attrSetType)
        {
            if (TryGetAttributeSet(attrSetType, out _)) return;
            var setName = AttributeSets.AttributeSetName(attrSetType);
            _attributeSets.Add(setName, Activator.CreateInstance(attrSetType) as AttributeSet);

            var attrSet = _attributeSets[setName];
            foreach (var attr in attrSet.AttributeNames)
            {
                if (!_attributeAggregators.ContainsKey(attrSet[attr]))
                {
                    var attrAggt = new AttributeAggregator(attrSet[attr], _asc);
                    if (_asc.enabled) attrAggt.Enable();
                    _attributeAggregators.Add(attrSet[attr], attrAggt);
                }
            }

            attrSet.SetOwner(_asc);
        }
        
        public bool TryGetAttributeSet<T>(out T attributeSet) where T : AttributeSet
        {
            if (_attributeSets.TryGetValue(AttributeSets.AttributeSetName(typeof(T)), out var set))
            {
                attributeSet = (T)set;
                return true;
            }

            attributeSet = null;
            return false;
        }
        
        bool TryGetAttributeSet(Type attrSetType, out AttributeSet attributeSet)
        {
            if (_attributeSets.TryGetValue(AttributeSets.AttributeSetName(attrSetType), out var set))
            {
                attributeSet = set;
                return true;
            }

            attributeSet = null;
            return false;
        }

        public AttributeValue? GetAttributeValue(string attributeSet, string attributeName)
        {
            return _attributeSets[attributeSet].Attributes[attributeName].value;
        }
        
        public float GetAttributeCurrentValue(string attributeSet, string attributeName)
        {
            return _attributeSets[attributeSet].Attributes[attributeName].GetCurrentValue();
        }
        
        public void SetAttributeCurrentValue(string attributeSet, string attributeName, float newValue)
        {
            _attributeSets[attributeSet].Attributes[attributeName].SetCurrentValue(newValue);
            NotifyAttributeChanged( attributeSet,  attributeName, newValue);
        }
        
        public void SetAttributeBaseValue(string attributeSet, string attributeName, float newValue)
        {
            _attributeSets[attributeSet].Attributes[attributeName].SetBaseValue(newValue);
        }
        
        public void NotifyAttributeChanged(string attributeSet, string attributeName, float newValue)
        {
            _attributeSets[attributeSet].Attributes[attributeName].SetCurrentValue(newValue);
        }

        private void OnEffectAdded()
        {
            _effectSpecsSnapshot.AddRange(_asc.EffectSystem.GetAllEffects());
            
            _effectSpecsSnapshot.Clear();
        }

        private void UpdateAttributes(List<EffectSpec> effects)
        {
            foreach (var (k,v) in _attributeSets)
            {
                foreach (var attribute in v.Attributes.Values)
                {
                    UpdateAttribute(attribute);
                } 
            }
        }

        private void UpdateAttribute(AttributeBase attribute)
        {
            
        }
    }
}