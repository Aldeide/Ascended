using System;
using System.Collections.Generic;
using System.Reflection;
using GraphProcessor;

namespace AbilityGraph.Runtime.Nodes.Base
{
    [Serializable]
    public abstract class ExecutableNode : AbilityNode, IExecutableNode
    {
        [Input(name = "Executed", allowMultiple = true)]
        public ExecutableLink Executed;

        public abstract IEnumerable<ExecutableNode> GetExecutedNodes();

        public override FieldInfo[] GetNodeFields()
        {
            var fields = base.GetNodeFields();
            Array.Sort(fields, FieldOrderComparer.Instance);
            return fields;
        }

        /// <summary>
        /// Static comparer so <see cref="GetNodeFields"/> never allocates a delegate.
        /// Ensures the "Executed" input pin is always rendered at the top.
        /// </summary>
        private sealed class FieldOrderComparer : IComparer<FieldInfo>
        {
            public static readonly FieldOrderComparer Instance = new FieldOrderComparer();
            private FieldOrderComparer() { }

            public int Compare(FieldInfo x, FieldInfo y)
            {
                bool xIsExecuted = x?.Name == nameof(Executed);
                bool yIsExecuted = y?.Name == nameof(Executed);
                if (xIsExecuted == yIsExecuted) return 0;
                return xIsExecuted ? -1 : 1;
            }
        }
    }
}
