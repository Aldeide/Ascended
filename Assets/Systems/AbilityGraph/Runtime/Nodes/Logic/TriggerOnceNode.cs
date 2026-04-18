using System;
using System.Collections.Generic;
using System.Linq;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Logic
{
    [Serializable, NodeMenuItem("Logic/Trigger Once")]
    public class TriggerOnceNode : ExecutableNode
    {
        [Input(name = "Reset")]
        public ExecutableLink Reset;

        [Output(name = "Out")]
        public ExecutableLink Out;

        private bool _triggered;

        public override IEnumerable<ExecutableNode> GetExecutedNodes()
        {
            if (!_triggered)
            {
                _triggered = true;
                return outputPorts.FirstOrDefault(p => p.fieldName == nameof(Out))
                    ?.GetEdges().Select(e => e.inputNode as ExecutableNode);
            }
            return Enumerable.Empty<ExecutableNode>();
        }

        public void ResetTrigger()
        {
            _triggered = false;
        }
    }
}
