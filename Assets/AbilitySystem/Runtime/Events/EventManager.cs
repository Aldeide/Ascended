using System;
using System.Collections.Generic;

namespace AbilitySystem.Runtime.Events
{
    public class EventManager
    {
        private readonly Dictionary<Type, Delegate> _typeToHandlers = new();

        public void Subscribe(Type type, Action<GameplayEvent> handler)
        {
            if (!_typeToHandlers.TryAdd(type, handler))
            {
                _typeToHandlers[type] =
                    Delegate.Combine(_typeToHandlers[type], handler);
            }
        }

        public void Unsubscribe(Type type,Action<GameplayEvent> handler)
        {
            if (_typeToHandlers.ContainsKey(type))
            {
                _typeToHandlers[type] = (Action<GameplayEvent>)_typeToHandlers[type] - handler;
            }
        }

        public void TriggerEvent(GameplayEvent gameEvent)
        {
            var actualType = gameEvent.EventType;
            if (!_typeToHandlers.TryGetValue(actualType, out var d)) return;
            if (d is Action<GameplayEvent> callback)
            {
                callback(gameEvent);
            }
        }
    }
}