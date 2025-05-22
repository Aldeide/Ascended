using System;
using AbilitySystem.Runtime.Events;

namespace AbilitySystem.Test.Utilities
{
    public class TestGameplayEvent : GameplayEvent   {
        public TestGameplayEvent(TestGameplayEventArgs arguments) : base(arguments)
        {
        }
    }

    public class TestGameplayEventArgs : EventArgs
    {
        
    }
    
    [Serializable]
    public class TestGameplayEventType : GameplayEventType
    {
        public override Type EventType => typeof(TestGameplayEvent);
    }
}