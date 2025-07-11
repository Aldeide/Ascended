using System;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Primitives
{
    [Serializable]
    [NodeMenuItem("Primitives/Float")]
    public class FloatNode : BaseNode {
        [Output("Out")] public float Output;

        [Input("In")] public float Input;

        public override string name => "Float";

        protected override void Process() {
            Output = Input;
        }
    }
}