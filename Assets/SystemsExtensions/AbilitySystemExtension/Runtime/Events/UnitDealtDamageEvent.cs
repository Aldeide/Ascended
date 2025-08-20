using System;
using AbilitySystem.Runtime.Events;

namespace AbilitySystemExtension.Runtime.Events
{
    [Serializable]
    public class UnitDealtDamageEvent : GameplayEvent
    {
        public UnitDealtDamageEvent(UnitDealtDamageEventArgs arguments) : base(arguments)
        {
        }
    }
    
    [Serializable]
    public class UnitDealtDamageEventArgs : EventArgs
    {
        public float Amount { get; private set;}
        
        public UnitDealtDamageEventArgs(float amount)
        {
            Amount = amount;
        }
    }

    [Serializable]
    public class UnitDealtDamageEventType : GameplayEventType
    {
        public override Type EventType => typeof(UnitDealtDamageEvent);
    }
}