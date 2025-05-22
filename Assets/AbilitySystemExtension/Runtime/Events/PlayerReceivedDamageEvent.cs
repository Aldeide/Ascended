using System;
using AbilitySystem.Runtime.Events;

namespace AbilitySystemExtension.Runtime.Events
{
    [Serializable]
    public class PlayerReceivedDamageEvent : GameplayEvent
    {
        public PlayerReceivedDamageEvent(PlayerReceivedDamageEventArgs arguments) : base(arguments)
        {
        }
    }

    [Serializable]
    public class PlayerReceivedDamageEventArgs : EventArgs
    {
        public float Amount { get; private set;}
        
        public PlayerReceivedDamageEventArgs(float amount)
        {
            Amount = amount;
        }
    }

    [Serializable]
    public class PlayerReceivedDamageEventType : GameplayEventType
    {
        public override Type EventType => typeof(PlayerReceivedDamageEvent);
    }
}