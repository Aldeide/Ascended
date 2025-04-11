using System;
using FishNet.Object;
using Systems.AbilitySystem.Attributes;
using Systems.AbilitySystem.Authoring;
using Systems.AbilitySystem.Effects;
using Systems.AbilitySystem.Effects.Modifiers;
using Systems.AbilitySystem.Tags;
using Systems.AbilitySystem.Util;
using Systems.Attributes;
using Systems.Development;
using UnityEngine;

namespace Systems.AbilitySystem.Components
{
    public class AbilitySystemComponent : NetworkBehaviour
    {
        public AbilitySystemPreset Preset;
        
        public AttributeSystem AttributesSystem;
        public TagSystem TagSystem;
        public EffectSystem EffectSystem;
        
        public DevelopmentComponent DevelopmentComponent;
        
        public void Start()
        {
            AttributesSystem = new AttributeSystem(this);
            TagSystem = new TagSystem(this);
            EffectSystem = new EffectSystem(this);
            
            AttributesSystem.Initialise(this);
            EffectSystem.Initialise(this);
            DevelopmentComponent = GetComponent<DevelopmentComponent>();
            DevelopmentComponent.Initialise(this);
            
            if (Preset != null) InitialiseWithPreset();
            
        }

        [Server]
        public void Update()
        {
            Tick();
        }

        public void InitialiseWithPreset()
        {
            foreach (var attributeSet in Preset.AttributeSets)
            {
                Type type = ReflectionUtil.GetAttributeSetType(attributeSet);
                AttributesSystem.AddAttributeSet(type);
            }
        }

        public void Tick()
        {
            EffectSystem.Tick();
        }

        public void AddEffect(EffectSpec effectSpec)
        {
            EffectSystem.AddEffectSpec(this, effectSpec);
        }

        public bool HasAllTags(GameplayTagSet tags)
        {
            return TagSystem.HasAllTags(tags);
        }
        
        public bool HasAnyTags(GameplayTagSet tags)
        {
            return TagSystem.HasAnyTags(tags.Tags);
        }

        public AttributeValue? GetAttributeValue(string attributeSet, string attributeName)
        {
            return AttributesSystem.GetAttributeValue(attributeSet, attributeName);
        }

        public void ApplyModifierFromInstantGameplayEffect(EffectSpec instantEffectSpec)
        {
            foreach (var modifier in instantEffectSpec.Modifiers)
            {
                var splits = modifier.attributeName.Split(".");
                var attributeSet = splits[0];
                var attributeName = splits[1];
                
                var attributeValue = GetAttributeValue(attributeSet, attributeName);
                if (attributeValue == null) continue;
                var magnitude = modifier.CalculateModifier(instantEffectSpec);
                var baseValue = attributeValue.Value.BaseValue;
                switch (modifier.operation)
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
                AttributesSystem.SetAttributeBaseValue(attributeSet, attributeName, baseValue);
            }
        }
        
    }
}