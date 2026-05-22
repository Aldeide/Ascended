using AbilitySystem.Scripts;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace AISystem.Runtime.Sensors
{
    public enum AttributeComparisonType
    {
        LessThan,
        GreaterThan,
        RatioLessThan,
        RatioGreaterThan
    }

    public class AttributeSensor : LocalWorldSensorBase
    {
        public string AttributeName { get; set; }
        public string MaxAttributeName { get; set; }
        public AttributeComparisonType Comparison { get; set; } = AttributeComparisonType.LessThan;
        public float Threshold { get; set; }

        public override void Created() {}

        public override void Update() {}

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            var asc = agent.Transform.GetComponent<AbilitySystemComponent>();
            if (asc == null || !asc.IsInitialized || string.IsNullOrEmpty(AttributeName))
                return false;

            var attribute = asc.AbilitySystem.AttributeSetManager.GetAttribute(AttributeName);
            if (attribute == null)
                return false;

            var val = attribute.CurrentValue;

            switch (Comparison)
            {
                case AttributeComparisonType.LessThan:
                    return val < Threshold;
                case AttributeComparisonType.GreaterThan:
                    return val > Threshold;
                case AttributeComparisonType.RatioLessThan:
                    if (string.IsNullOrEmpty(MaxAttributeName)) return false;
                    var maxAttr = asc.AbilitySystem.AttributeSetManager.GetAttribute(MaxAttributeName);
                    if (maxAttr == null || maxAttr.CurrentValue == 0f) return false;
                    return (val / maxAttr.CurrentValue) < Threshold;
                case AttributeComparisonType.RatioGreaterThan:
                    if (string.IsNullOrEmpty(MaxAttributeName)) return false;
                    var maxAttr2 = asc.AbilitySystem.AttributeSetManager.GetAttribute(MaxAttributeName);
                    if (maxAttr2 == null || maxAttr2.CurrentValue == 0f) return false;
                    return (val / maxAttr2.CurrentValue) > Threshold;
                default:
                    return false;
            }
        }
    }
}
