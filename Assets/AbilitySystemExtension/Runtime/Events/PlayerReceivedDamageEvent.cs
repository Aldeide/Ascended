using System;
using AbilitySystem.Runtime.Events;

namespace AbilitySystemExtension.Runtime.Events
{
    [Serializable]
    public class PlayerReceivedDamageEvent : GameplayEvent<PlayerReceivedDamageEventArgs>
    {
        public PlayerReceivedDamageEvent(PlayerReceivedDamageEventArgs arguments) : base(
            typeof(PlayerReceivedDamageEvent), arguments)
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
}