using System;
using System.Collections.Generic;
using System.Reflection;

namespace AbilityGraph.Runtime.Nodes
{
    public interface IExecutableNode
    {
        IEnumerable<ExecutableNode>	GetExecutedNodes();
        FieldInfo[] GetNodeFields(); // Provide a custom order for fields (so conditional links are always at the top of the node)
    }
}