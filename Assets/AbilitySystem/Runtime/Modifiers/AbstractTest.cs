using System;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public abstract class AbstractTest
    {
        public abstract float Calculate();

        public abstract string SaySomething();
    }
}