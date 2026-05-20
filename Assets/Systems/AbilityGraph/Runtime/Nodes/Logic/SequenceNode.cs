using System;
using System.Collections.Generic;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Logic
{
    [Serializable, NodeMenuItem("Logic/Sequence")]
    public class SequenceNode : ExecutableNode
    {
        [Output(name = "Then 0")]
        public ExecutableLink Then0;

        [Output(name = "Then 1")]
        public ExecutableLink Then1;

        [Output(name = "Then 2")]
        public ExecutableLink Then2;

        private NodePort _port0;
        private NodePort _port1;
        private NodePort _port2;

        public override void Initialise(GraphContext context)
        {
            base.Initialise(context);
            foreach (var port in outputPorts)
            {
                if (port.fieldName == nameof(Then0)) _port0 = port;
                else if (port.fieldName == nameof(Then1)) _port1 = port;
                else if (port.fieldName == nameof(Then2)) _port2 = port;
            }
        }

        public override IEnumerable<ExecutableNode> GetExecutedNodes()
        {
            // Pushing order: Stack is LIFO (last-in, first-out).
            // To execute: Then 0, then Then 1, then Then 2, we must return them in reverse order:
            // Then 2, then Then 1, then Then 0.

            if (_port2 != null)
            {
                foreach (var edge in _port2.GetEdges())
                {
                    if (edge.inputNode is ExecutableNode exec)
                        yield return exec;
                }
            }

            if (_port1 != null)
            {
                foreach (var edge in _port1.GetEdges())
                {
                    if (edge.inputNode is ExecutableNode exec)
                        yield return exec;
                }
            }

            if (_port0 != null)
            {
                foreach (var edge in _port0.GetEdges())
                {
                    if (edge.inputNode is ExecutableNode exec)
                        yield return exec;
                }
            }
        }
    }
}
