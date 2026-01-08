using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;

namespace AbilitySystem.Runtime.Abilities.Cooldowns
{
    /// <summary>
    /// Represents a cooldown mechanism driven by a specific attribute.
    /// </summary>
    /// <remarks>
    /// The cooldown duration for abilities is determined dynamically based on the value of an attribute.
    /// This class leverages a GameplayTagQuery to filter relevant modifiers affecting the provided attribute.
    /// </remarks>
    [Serializable]
    public class AttributeDrivenAbilityCooldown : AbilityCooldown
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeChoices")]
        public string CooldownAttribute = "";

        public TagQuery TagQuery;
        
        private readonly List<Tuple<Effect, Modifier>> _relevantModifiers = new();

        public override bool Activate(IAbilitySystem owner)
        {
            if (!CanActivate(owner)) return false;
            
            var effect = CooldownEffect.ToEffect(owner, owner);
            effect.Activate();
            effect.Duration = Calculate(owner);
            owner.EffectManager.AddEffect(effect);
            return true;
        }
        
        public override float Calculate(IAbilitySystem owner)
        {
            var splits = CooldownAttribute.Split('.');
            var attributeName = splits[1];
            var aggregator = owner.AttributeSetManager.GetAggregator(attributeName);
            var modifiers = aggregator.GetModifiers();
            _relevantModifiers.Clear();
            foreach (var modifier in modifiers.Where(modifier =>
                         TagQuery.MatchesTags(modifier.Effect.Definition.AssetTags)))
            {
                _relevantModifiers.Add(new Tuple<Effect, Modifier>(modifier.Effect, modifier.Modifier));
            }
            return ApplyModifiers();
        }
        
        private float ApplyModifiers()
        {
            var value = CooldownEffect.DurationSeconds;
            var additive = 0f;
            var multiplicative = 1f;
            var overrideValue = 0f;
            bool hasOverride = false;
            foreach (var modifier in _relevantModifiers)
            {
                switch (modifier.Item2.Operation)
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