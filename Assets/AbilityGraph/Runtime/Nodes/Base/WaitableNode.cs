using System;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Base
{
    [System.Serializable]
    /// <summary>
    /// This class represent a waitable node which invokes another node after a time/frame
    /// </summary>
    public abstract class WaitableNode : LinearExecutableNode
    {
        [Output(name = "Execute After")]
        public ExecutableLink executeAfter;

        protected void ProcessFinished()
        {
            onProcessFinished.Invoke(this);
        }

        [HideInInspector]
        public Action<WaitableNode> onProcessFinished;

        public IEnumerable< ExecutableNode > GetExecuteAfterNodes()
        {
            return outputPorts.FirstOrDefault(n => n.fieldName == nameof(executeAfter))
                .GetEdges().Select(e => e.inputNode as ExecutableNode);
        }
    }
}