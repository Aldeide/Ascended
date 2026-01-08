using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Modifiers;
using GraphProcessor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Attributes/GetAttribute")]
    public class GetAttributeNode : AbilityNode
    {
        [Output("Current Value")] public float currentValue;
        [Output("Base Value")] public float baseValue;

        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        public string attributeFullName;
        
        protected override void Process()
        {
            var splits = attributeFullName.Split(".");
            var attributeSetName = splits[0];
            var attributeName = splits[1];
            var attribute = Owner.AttributeSetManager.GetAttribute(attributeName);
            currentValue = attribute.CurrentValue;
            baseValue = attribute.BaseValue;
        }
    }
}