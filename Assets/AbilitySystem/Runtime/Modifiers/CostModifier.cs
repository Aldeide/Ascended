using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public class CostModifier : Modifier
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        public string costMetaAttribute;

        public float baseCost;

        [ValueDropdown("@DropdownValuesUtil.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] modifierTags;

        private string _attributeSet;
        private string _attributeName;
        private List<Tuple<Effect, Modifier>> _relevantModifiers = new();

        public override float Calculate(Effect effect)
        {
            var splits = costMetaAttribute.Split(".");
            _attributeSet = splits[0];
            _attributeName = splits[1];
            var asc = effect.Owner;
            var aggregator = asc.AttributeSetManager.GetAggregator(_attributeName);
            var modifiers = aggregator.GetModifiers();

            _relevantModifiers.Clear();
            foreach (var modifier in from modifier in modifiers
                     from tag in modifierTags
                     where modifier.Effect.Definition.AssetTags.Any(e => e.IsAncestorOf(tag) || e.Equals(tag))
                     select modifier)
            {
                _relevantModifiers.Add(new Tuple<Effect, Modifier>(modifier.Effect, modifier.Modifier));
            }

            return ApplyModifiers();
        }

        private float ApplyModifiers()
        {
            var value = baseCost;
            var additive = 0f;
            var multiplicative = 1f;
            var overrideValue = 0f;
            bool hasOverride = false;
            foreach (var modifier in _relevantModifiers)
            {
                switch (modifier.Item2.operation)
                {
                    case EffectOperation.Additive:
                        additive += modifier.Item2.Calculate(modifier.Item1);
                        break;
                    case EffectOperation.Subtractive:
                        additive -= modifier.Item2.Calculate(modifier.Item1);
                        break;
                    case EffectOperation.Multiplicative:
                        multiplicative *= modifier.Item2.Calculate(modifier.Item1);
                        break;
                    case EffectOperation.Divisive:
                        multiplicative /= modifier.Item2.Calculate(modifier.Item1);
                        break;
                    case EffectOperation.Override:
                        overrideValue = modifier.Item2.Calculate(modifier.Item1);
                        hasOverride = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (hasOverride) return overrideValue;
            return (value + additive) * multiplicative;
        }
    }
}