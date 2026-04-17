using AbilityGraph.Runtime.Nodes.Base;
using AbilityGraph.Runtime.Nodes.Base;
using System;
using GraphProcessor;
using UnityEngine.Serialization;

namespace AbilityGraph.Runtime.Nodes.Primitives
{
    [Serializable]
    [NodeMenuItem("Primitives/String")]
    public class StringNode : AbilityNode
    {
        public string Value;

        [Output("String")] public string Output;

        protected override void Process()
        {
            Output = Value;
        }
    }
}
