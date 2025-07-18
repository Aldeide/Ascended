using System;
using AbilitySystem.Runtime.Effects;

namespace Plugins.AbilitySystem.Runtime.ScalableValue
{
    [Serializable]
    public abstract class ScalableValue
    {
        public abstract float GetValue(Effect effect);
    }
}