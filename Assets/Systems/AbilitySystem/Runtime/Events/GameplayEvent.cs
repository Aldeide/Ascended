using System;

namespace AbilitySystem.Runtime.Events
{
    [Serializable]
    public abstract class GameplayEvent
    {
        public Type EventType { get; private set; }
        public EventArgs Arguments { get; private set; }

        protected GameplayEvent(EventArgs arguments)
        {
            EventType = GetType();
            Arguments = arguments;
        }
    }
    
    public abstract class GameplayEventType {
        public abstract Type EventType { get; }
    }
}