using System;
using System.Collections.Generic;
using System.Linq;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Logic
{
    [Serializable, NodeMenuItem("Logic/Do Once")]
    public class DoOnceNode : ExecutableNode
    {
        [Input(name = "Reset")]
        public ExecutableLink Reset;

        [Output(name = "Executes")]
        public ExecutableLink Executes;

        [Input(name = "Start Closed")]
        public bool StartClosed;

        private bool _triggered;
        private bool _isInitialized;

        public override void Initialise(GraphContext context)
        {
            base.Initialise(context);
            if (!_isInitialized)
            {
                _triggered = StartClosed;
                _isInitialized = true;
            }
        }

        public override IEnumerable<ExecutableNode> GetExecutedNodes()
        {
            if (!_triggered)
            {
                _triggered = true;
                var port = outputPorts.FirstOrDefault(p => p.fieldName == nameof(Executes));
                if (port != null)
                {
                    foreach (var edge in port.GetEdges())
                    {
                        if (edge.inputNode is ExecutableNode exec)
                            yield return exec;
                    }
                }
            }
        }

        public void ResetTrigger()
        {
            _triggered = false;
        }
    }
}
