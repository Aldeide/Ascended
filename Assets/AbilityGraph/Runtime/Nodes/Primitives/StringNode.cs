using System;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Primitives
{
    [Serializable]
    [NodeMenuItem("Primitives/String")]
    public class StringNode : BaseNode
    {
        public string value;

        [Output("String")] public string output;

        protected override void Process()
        {
            output = value;
        }
    }
}