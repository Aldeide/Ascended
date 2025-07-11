using System;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Primitives
{
    [Serializable]
    [NodeMenuItem("Primitives/Integer")]
    public class IntNode : BaseNode {
        
        [Output("Out")] public int Output;
        [Input("In")] public int Input;

        public override string name => "Integer";

        protected override void Process() {
            Output = Input;
        }
    }
}