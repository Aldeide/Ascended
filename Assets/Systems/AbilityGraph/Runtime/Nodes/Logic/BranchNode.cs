using System;
using System.Collections.Generic;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Logic
{
    [Serializable, NodeMenuItem("Logic/Branch")]
    public class BranchNode : Nodes.Base.ExecutableNode, Nodes.IExecutableNode
    {
        [Input(name = "Condition")]
        public bool Condition;

        [Output(name = "ExecutesIfTrue")]
        public Nodes.Base.ExecutableLink ExecutesIfTrue;

        [Output(name = "ExecutesIfFalse")]
        public Nodes.Base.ExecutableLink ExecutesIfFalse;

        // Cached at Initialise() time.
        private NodePort _truePort;
        private NodePort _falsePort;

        public override void Initialise(GraphContext context)
        {
            base.Initialise(context);
            foreach (var port in outputPorts)
            {
                if (port.fieldName == nameof(ExecutesIfTrue))  _truePort  = port;
                else if (port.fieldName == nameof(ExecutesIfFalse)) _falsePort = port;
            }
        }

        public override IEnumerable<Nodes.Base.ExecutableNode> GetExecutedNodes()
        {
            var port = Condition ? _truePort : _falsePort;
            if (port == null) yield break;
            foreach (var edge in port.GetEdges())
            {
                if (edge.inputNode is Nodes.Base.ExecutableNode exec)
                    yield return exec;
            }
        }
    }
}
