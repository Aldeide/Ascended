using System;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Serialization;

namespace AbilityGraph.Runtime.Nodes.Primitives
{
    [Serializable]
    [NodeMenuItem("Primitives/Vector3")]
    public class Vector3Node : BaseNode {
        [Output("Out")] public Vector3 Output;
        [Input("In"), ShowAsDrawer] public Vector3 Input;

        public override string name => "Vector3";

        protected override void Process() {
            Output = Input;
        }
    }
}