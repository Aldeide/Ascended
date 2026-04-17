using AbilityGraph.Runtime.Nodes.Base;
using System;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Primitives
{
    [Serializable]
    [NodeMenuItem("Primitives/Vector2")]
    public class Vector2Node : AbilityNode {
        [Output("Out")] public Vector2 Output;
        [Input("In"), ShowAsDrawer] public Vector2 Input;

        public override string name => "Vector2";

        protected override void Process() {
            Output = Input;
        }
    }
}