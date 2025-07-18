using System;
using AbilitySystem.Runtime.Effects;
using UnityEngine;

namespace Plugins.AbilitySystem.Runtime.ScalableValue
{
    [Serializable]
    public class ValuePerLevel : ScalableValue
    {
        public AnimationCurve Curve;

        public override float GetValue(Effect effect)
        {
            if (Curve == null) return 0;
            return Curve.Evaluate(effect.Level);
        }
    }
}