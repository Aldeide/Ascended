using System.Linq;
using AbilityGraph.Runtime.Nodes.Abilities;
using AbilitySystem.Runtime.Utilities;
using GraphProcessor;
using UnityEngine.UIElements;

namespace AbilityGraph.Editor.Nodes
{
    [NodeCustomEditor(typeof(GetAttributeNode))]
    public class GetAttributeNodeView : BaseNodeView
    {
        public override void Enable() {
            var node = nodeTarget as GetAttributeNode;
            var attributeChoices = DropdownValuesUtil.AttributeChoices;
            
            var dropdown = new DropdownField() {
                choices = attributeChoices.ToList()
            };

            if (node != null)
            {
                node.onProcessed += () => dropdown.value = node.attributeFullName;

                dropdown.RegisterValueChangedCallback(v =>
                {
                    owner.RegisterCompleteObjectUndo("Updated dropdown input");
                    node.attributeFullName = v.newValue;
                });
            }

            controlsContainer.Add(dropdown);
        }
    }
}