using System;
using AbilitySystem.Runtime.Events;

namespace AbilitySystemExtension.Runtime.Events
{
    public class AbilityStartEvent : GameplayEvent
    {
        public AbilityStartEvent(AbilityStartEventArgs arguments) : base(arguments)
        {
        }
    }
    
    [Serializable]
    public class AbilityStartEventArgs : EventArgs
    {
        public string AbilityName { get; private set;}
        
        public AbilityStartEventArgs(string abilityName)
        {
            AbilityName = abilityName;
        }
    }

    [Serializable]
    public class AbilityStartEventType : GameplayEventType
    {
        public override Type EventType => typeof(PlayerDeathEvent);
    }
}