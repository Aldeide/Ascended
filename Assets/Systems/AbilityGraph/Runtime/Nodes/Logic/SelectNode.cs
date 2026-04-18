using System;
using System.Collections.Generic;
using System.Linq;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Logic
{
    [Serializable, NodeMenuItem("Logic/Select")]
    public class SelectNode : AbilityNode
    {
        [Input(name = "Condition")]
        public bool Condition;

        [Input(name = "True Value")]
        public object TrueValue;

        [Input(name = "False Value")]
        public object FalseValue;

        [Output(name = "Result")]
        public object Result;

        [SerializeField, HideInInspector]
        public SerializableType SelectedType = new SerializableType(typeof(float));

        public override string name => $"Select ({SelectedType.type?.Name ?? "Object"})";

        protected override void Process()
        {
            Result = Condition ? TrueValue : FalseValue;
        }

        [CustomPortBehavior(nameof(TrueValue))]
        IEnumerable<PortData> TrueValuePortBehavior(List<SerializableEdge> edges)
        {
            UpdateSelectedType(edges);
            yield return GetPortData();
        }

        [CustomPortBehavior(nameof(FalseValue))]
        IEnumerable<PortData> FalseValuePortBehavior(List<SerializableEdge> edges)
        {
            UpdateSelectedType(edges);
            yield return GetPortData();
        }

        [CustomPortBehavior(nameof(Result))]
        IEnumerable<PortData> ResultPortBehavior(List<SerializableEdge> edges)
        {
            UpdateSelectedType(edges);
            yield return GetPortData();
        }

        private void UpdateSelectedType(List<SerializableEdge> edges)
        {
            var firstEdge = edges.FirstOrDefault();
            if (firstEdge != null)
            {
                Type newType = null;
                if (firstEdge.inputNode == this) // Input port
                {
                    newType = firstEdge.outputPort.portData.displayType ?? firstEdge.outputPort.fieldInfo.FieldType;
                }
                else // Output port
                {
                    newType = firstEdge.inputPort.portData.displayType ?? firstEdge.inputPort.fieldInfo.FieldType;
                }

                if (newType != null && newType != SelectedType.type && newType != typeof(object))
                {
                    SelectedType.type = newType;
                    // Trigger update on other ports to propagate the type
                    // Actually, NodeGraphProcessor handles this if we are in the middle of a PortBehavior call
                }
            }
        }

        private PortData GetPortData()
        {
            return new PortData
            {
                displayType = SelectedType.type ?? typeof(object),
                identifier = "0",
            };
        }
    }
}
