using AbilitySystem.Runtime.Effects;

namespace AbilitySystem.Runtime.Modifiers
{
    public struct ModifierData
    {
        public float Magnitude;
        public EffectOperation Operation;

        public ModifierData(float magnitude, EffectOperation operation)
        {
            Magnitude = magnitude;
            Operation = operation;
        }
    }
}
