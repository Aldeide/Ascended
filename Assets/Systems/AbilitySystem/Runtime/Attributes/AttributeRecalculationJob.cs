using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Runtime.Effects;

namespace AbilitySystem.Runtime.Attributes
{
    [BurstCompile]
    public struct AttributeRecalculationJob : IJobParallelFor
    {
        public NativeArray<AttributeState> States;
        [ReadOnly] public NativeArray<ModifierData> AllModifiers;
        [ReadOnly] public NativeArray<int2> ModifierRanges; // x = start index, y = count

        public void Execute(int index)
        {
            var state = States[index];
            var range = ModifierRanges[index];

            if (range.y == 0)
            {
                state.CurrentValue = math.clamp(state.BaseValue, state.MinValue, state.MaxValue);
                States[index] = state;
                return;
            }

            float additive = 0;
            float multiplier = 1.0f;
            float overrideValue = 0;
            bool hasOverride = false;

            for (int i = 0; i < range.y; i++)
            {
                var mod = AllModifiers[range.x + i];
                switch (mod.Operation)
                {
                    case EffectOperation.Additive:
                        additive += mod.Magnitude;
                        break;
                    case EffectOperation.Subtractive:
                        additive -= mod.Magnitude;
                        break;
                    case EffectOperation.Multiplicative:
                        multiplier *= mod.Magnitude;
                        break;
                    case EffectOperation.Divisive:
                        if (math.abs(mod.Magnitude) > 0.0001f)
                            multiplier /= mod.Magnitude;
                        break;
                    case EffectOperation.Override:
                        overrideValue = mod.Magnitude;
                        hasOverride = true;
                        break;
                }
            }

            if (hasOverride)
            {
                state.CurrentValue = overrideValue;
            }
            else
            {
                state.CurrentValue = (state.BaseValue + additive) * multiplier;
            }

            state.CurrentValue = math.clamp(state.CurrentValue, state.MinValue, state.MaxValue);
            States[index] = state;
        }
    }
}
