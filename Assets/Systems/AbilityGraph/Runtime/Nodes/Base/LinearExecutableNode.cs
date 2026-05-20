using System;
using System.Collections.Generic;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Base
{
    [Serializable]
    public abstract class LinearExecutableNode : ExecutableNode, IExecutableNode
    {
        [Output(name = "Executes")]
        public ExecutableLink Executes;

        // Cached at Initialise() time to avoid per-execution LINQ port lookup.
        private NodePort _executesPort;

        public override void Initialise(GraphContext context)
        {
            base.Initialise(context);
            CachePorts();
        }

        protected virtual void CachePorts()
        {
            _executesPort = null;
            foreach (var port in outputPorts)
            {
                if (port.fieldName == nameof(Executes))
                {
                    _executesPort = port;
                    break;
                }
            }
        }

        public override IEnumerable<ExecutableNode> GetExecutedNodes()
        {
            if (_executesPort == null) yield break;
            foreach (var edge in _executesPort.GetEdges())
            {
                if (edge.inputNode is ExecutableNode execNode)
                    yield return execNode;
            }
        }
    }
}
