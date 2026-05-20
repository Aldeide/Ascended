using System;
using System.Collections.Generic;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Base
{
    /// <summary>
    /// A node that suspends graph execution until an async operation completes,
    /// then resumes via the "Execute After" output port.
    /// </summary>
    [Serializable]
    public abstract class WaitableNode : LinearExecutableNode
    {
        [Output(name = "Execute After")]
        public ExecutableLink ExecuteAfter;

        // Cached at Initialise() time.
        private NodePort _executeAfterPort;

        public Action<WaitableNode> onProcessFinished;

        protected void ProcessFinished()
        {
            onProcessFinished?.Invoke(this);
        }

        public override void Initialise(GraphContext context)
        {
            base.Initialise(context);
            // CachePorts in base handles Executes; we handle ExecuteAfter here.
        }

        protected override void CachePorts()
        {
            base.CachePorts();
            _executeAfterPort = null;
            foreach (var port in outputPorts)
            {
                if (port.fieldName == nameof(ExecuteAfter))
                {
                    _executeAfterPort = port;
                    break;
                }
            }
        }

        public IEnumerable<ExecutableNode> GetExecuteAfterNodes()
        {
            if (_executeAfterPort == null) yield break;
            foreach (var edge in _executeAfterPort.GetEdges())
            {
                if (edge.inputNode is ExecutableNode execNode)
                    yield return execNode;
            }
        }
    }
}
