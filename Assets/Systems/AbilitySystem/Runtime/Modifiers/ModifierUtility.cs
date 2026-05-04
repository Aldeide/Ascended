using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.Effects;

namespace AbilitySystem.Runtime.Modifiers
{
    public static class ModifierUtility
    {
        public static float ApplyModifiers(float baseValue, IEnumerable<Tuple<Effect, Modifier>> modifiers)
        {
            var value = baseValue;
            var additive = 0f;
            var multiplicative = 1f;
            var overrideValue = 0f;
            var hasOverride = false;
            foreach (var modifier in modifiers)
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
