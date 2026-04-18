using System;
using System.Collections.Generic;
using System.Linq;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Logic
{
    [Serializable, NodeMenuItem("Logic/Chance")]
    public class ChanceNode : ExecutableNode
    {
        [Input(name = "Probability (0-1)")]
        public float Probability = 0.5f;

        [Output(name = "Success")]
        public ExecutableLink Success;

        [Output(name = "Failure")]
        public ExecutableLink Failure;

        public override IEnumerable<ExecutableNode> GetExecutedNodes()
        {
            float randomValue = UnityEngine.Random.value;
            string portName = randomValue <= Probability ? nameof(Success) : nameof(Failure);

            return outputPorts.FirstOrDefault(p => p.fieldName == portName)
                ?.GetEdges().Select(e => e.inputNode as ExecutableNode);
        }
    }
}
