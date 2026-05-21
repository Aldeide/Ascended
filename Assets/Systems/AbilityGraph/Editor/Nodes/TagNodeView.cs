using System.Linq;
using AbilityGraph.Runtime.Nodes.Primitives;
using GameplayTags.Runtime;
using GraphProcessor;
using UnityEngine.UIElements;

namespace AbilityGraph.Editor.Nodes
{
    [NodeCustomEditor(typeof(TagNode))]
    public class TagNodeView : BaseNodeView
    {
        public override void Enable() {
            var node = nodeTarget as TagNode;
            
            // Get all choices from TagsDropdown.GameplayTagChoices
            var tagChoices = TagsDropdown.GameplayTagChoices
                .Select(choice => choice.Text)
                .ToList();

            var dropdown = new DropdownField() {
                label = "Tag",
                choices = tagChoices
            };

            if (node != null)
            {
                dropdown.value = node.Tag.Name ?? "";
                node.onProcessed += () => dropdown.value = node.Tag.Name ?? "";

                dropdown.RegisterValueChangedCallback(v =>
                {
                    owner.RegisterCompleteObjectUndo("Updated TagNode Tag");
                    node.Tag = new Tag(v.newValue);
                    NotifyNodeChanged();
                });
            }

            controlsContainer.Add(dropdown);
        }
    }
}
