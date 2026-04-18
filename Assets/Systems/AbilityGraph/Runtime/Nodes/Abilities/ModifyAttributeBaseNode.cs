using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Effects;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    public enum AttributeModificationType
    {
        Add,
        Set,
        Multiply
    }

    [Serializable, NodeMenuItem("Abilities/Modify Attribute Base")]
    public class ModifyAttributeBaseNode : LinearExecutableNode
    {
        public string AttributeName;
        public AttributeModificationType ModificationType = AttributeModificationType.Add;
        
        [Input(name = "Value")]
        public float Value;

        protected override void Process()
        {
            if (Owner == null || string.IsNullOrEmpty(AttributeName)) return;

            var attribute = Owner.AttributeSetManager.GetAttribute(AttributeName);
            if (attribute == null) return;

            float newValue = attribute.BaseValue;
            switch (ModificationType)
            {
                case AttributeModificationType.Add:
                    newValue += Value;
                    break;
                case AttributeModificationType.Set:
                    newValue = Value;
                    break;
                case AttributeModificationType.Multiply:
                    newValue *= Value;
                    break;
            }
            
            attribute.SetBaseValue(newValue);
        }
    }
}
