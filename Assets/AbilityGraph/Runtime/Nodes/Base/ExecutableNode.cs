using System;
using System.Collections.Generic;
using System.Reflection;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Base
{
    [Serializable]
    public abstract class ExecutableNode : BaseNode, IExecutableNode
    {
        [Input(name = "Executed", allowMultiple = true)]
        public ExecutableLink Executed;

        public abstract IEnumerable<ExecutableNode> GetExecutedNodes();

        public override FieldInfo[] GetNodeFields()
        {
            var fields = base.GetNodeFields();
            Array.Sort(fields, (f1, f2) => f1.Name == nameof(Executed) ? -1 : 1);
            return fields;
        }
    }
}