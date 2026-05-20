using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.AttributeSets;
using GraphProcessor;
using Sirenix.OdinInspector;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Attributes/Get Attribute Percent")]
    public class GetAttributePercentNode : AbilityNode
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        public string CurrentAttributeFullName;

        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        public string MaxAttributeFullName;

        [Output("Percent")]
        public float Percent;

        protected override void Process()
        {
            if (Owner == null || string.IsNullOrEmpty(CurrentAttributeFullName) || string.IsNullOrEmpty(MaxAttributeFullName))
            {
                Percent = 0f;
                return;
            }

            var splitsCurrent = CurrentAttributeFullName.Split('.');
            var splitsMax = MaxAttributeFullName.Split('.');
            if (splitsCurrent.Length < 2 || splitsMax.Length < 2)
            {
                Percent = 0f;
                return;
            }

            var currentAttr = Owner.AttributeSetManager.GetAttribute(splitsCurrent[1]);
            var maxAttr = Owner.AttributeSetManager.GetAttribute(splitsMax[1]);

            if (currentAttr != null && maxAttr != null && maxAttr.CurrentValue != 0f)
            {
                Percent = currentAttr.CurrentValue / maxAttr.CurrentValue;
            }
            else
            {
                Percent = 0f;
            }
        }
    }
}
