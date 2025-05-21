using System;

namespace AbilitySystem.Runtime.Events
{
    [Serializable]
    public abstract class GameplayEvent<T> where T : EventArgs
    {
        public Type Type { get; private set; }
        public T Arguments { get; private set; }

        protected GameplayEvent(Type type, T arguments)
        {
            Type = type;
            Arguments = arguments;
        }
    }
}