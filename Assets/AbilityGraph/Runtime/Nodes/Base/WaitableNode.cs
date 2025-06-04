using System;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Base
{
    
    /// <summary>
    /// This class represents a waitable node which invokes another node after a time/frame
    /// </summary>
    [Serializable]
    public abstract class WaitableNode : LinearExecutableNode
    {
        [Output(name = "Execute After")]
        public ExecutableLink ExecuteAfter;

        protected void ProcessFinished()
        {
            onProcessFinished.Invoke(this);
        }

        [HideInInspector]
        public Action<WaitableNode> onProcessFinished;

        public IEnumerable< ExecutableNode > GetExecuteAfterNodes()
        {
            return outputPorts.FirstOrDefault(n => n.fieldName == nameof(ExecuteAfter))
                .GetEdges().Select(e => e.inputNode as ExecutableNode);
        }
    }
}