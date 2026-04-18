using System;
using System.Collections.Generic;
using System.Linq;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Logic
{
    [Serializable, NodeMenuItem("Logic/Gate")]
    public class GateNode : ExecutableNode
    {
        [Input(name = "Open")]
        public ExecutableLink Open;

        [Input(name = "Close")]
        public ExecutableLink Close;

        [Input(name = "Toggle")]
        public ExecutableLink Toggle;

        [Output(name = "Out")]
        public ExecutableLink Out;

        public bool IsOpen = true;

        protected override void Process()
        {
            // If the process was triggered by an incoming data check (not execution)
            // we don't change state here. The execution logic is in GetExecutedNodes.
        }

        public override IEnumerable<ExecutableNode> GetExecutedNodes()
        {
            // This is a bit tricky with the current GraphRunner as it doesn't distinguish 
            // WHICH port triggered the execution easily without custom metadata in the runner.
            // However, we can treat the standard 'Executed' input as the one that checks the gate.
            
            if (IsOpen)
            {
                return outputPorts.FirstOrDefault(p => p.fieldName == nameof(Out))
                    ?.GetEdges().Select(e => e.inputNode as ExecutableNode);
            }
            return Enumerable.Empty<ExecutableNode>();
        }
        
        // Custom logic to handle the Open/Close/Toggle inputs would normally be done in the Runner 
        // or by making this node listen to specific execution events if the framework supported it.
        // For now, we'll keep it simple: it acts as a boolean check.
    }
}
