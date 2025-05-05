using System;
using System.Collections.Generic;

namespace AbilitySystem.Runtime.Events
{
    public class EventManager
    {
        private readonly Dictionary<Type, Delegate> _typeToHandlers = new();
        
        public void Subscribe<T>(Action<GameplayEvent<T>> handler)
        {
            var type = typeof(T);
            if (!_typeToHandlers.TryAdd(type, handler))
            {
                _typeToHandlers[type] = Delegate.Combine(_typeToHandlers[type], handler);
            }
        }

        public void Unsubscribe<T>(Action<GameplayEvent<T>> handler)
        {
            var type = typeof(T);
            if (_typeToHandlers.ContainsKey(type))
            {
                _typeToHandlers[type] = (Action<GameplayEvent<T>>)_typeToHandlers[type] - handler;
            }
        }
        
        public void TriggerEvent<T>(GameplayEvent<T> gameEvent)
        {
            if (!_typeToHandlers.TryGetValue(typeof(T), out var d)) return;
            if (d is Action<GameplayEvent<T>> callback)
            {
                callback(gameEvent);
            }
        }
    }
}