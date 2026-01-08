using System;
using GraphProcessor;
using UnityEngine.Serialization;

namespace AbilityGraph.Runtime.Nodes.Primitives
{
    [Serializable]
    [NodeMenuItem("Primitives/String")]
    public class StringNode : BaseNode
    {
        public string Value;

        [Output("String")] public string Output;

        protected override void Process()
        {
            Output = Value;
        }
    }
}