using System;
using AbilitySystem.Runtime.Events;

namespace AbilitySystemExtension.Runtime.Events
{
    [Serializable]
    public class PlayerDeathEvent : GameplayEvent
    {
        public PlayerDeathEvent(PlayerDeathEventArgs arguments) : base(arguments)
        {
        }
    }

    [Serializable]
    public class PlayerDeathEventArgs : EventArgs
    {
        public float Amount { get; private set;}
        
        public PlayerDeathEventArgs(float amount)
        {
            Amount = amount;
        }
    }

    [Serializable]
    public class PlayerDeathEventType : GameplayEventType
    {
        public override Type EventType => typeof(PlayerDeathEvent);
    }
}