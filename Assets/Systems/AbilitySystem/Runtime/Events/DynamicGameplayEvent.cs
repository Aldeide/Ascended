using System;
using GameplayTags.Runtime;

namespace AbilitySystem.Runtime.Events
{
    [Serializable]
    public class DynamicGameplayEvent : GameplayEvent
    {
        public Tag EventTag { get; set; }
        public float Magnitude { get; set; }

        public DynamicGameplayEvent(Tag tag, float magnitude) : base(EventArgs.Empty)
        {
            EventTag = tag;
            Magnitude = magnitude;
        }
    }
}
