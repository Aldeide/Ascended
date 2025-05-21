using System;
using AbilitySystem.Runtime.Events;

namespace AbilitySystem.Test.Utilities
{
    public class TestGameplayEvent : GameplayEvent<TestGameplayEventArgs>   {
        public TestGameplayEvent(TestGameplayEventArgs arguments) : base(typeof(TestGameplayEvent), arguments)
        {
        }
    }

    public class TestGameplayEventArgs : EventArgs
    {
        
    }
}