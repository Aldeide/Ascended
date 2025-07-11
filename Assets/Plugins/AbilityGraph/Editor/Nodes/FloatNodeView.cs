using AbilityGraph.Runtime.Nodes.Primitives;
using GraphProcessor;
using UnityEngine.UIElements;

namespace AbilityGraph.Editor.Nodes
{
    [NodeCustomEditor(typeof(FloatNode))]
    public class FloatNodeView : BaseNodeView {
        public override void Enable() {
            if (nodeTarget is FloatNode floatNode)
            {
                var floatField = new DoubleField {
                    value = floatNode.Input
                };

                floatNode.onProcessed += () => floatField.value = floatNode.Input;

                floatField.RegisterValueChangedCallback(v => {
                    owner.RegisterCompleteObjectUndo("Updated floatNode input");
                    floatNode.Input = (float)v.newValue;
                });

                controlsContainer.Add(floatField);
            }

            DrawDefaultInspector();
        }
    }
}