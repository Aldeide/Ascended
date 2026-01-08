using System;
using System.Threading.Tasks;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Utilities
{
    [Serializable, NodeMenuItem("Utilities/WaitForSeconds")]
    public class WaitNode : WaitableNode
    {
        public float Duration;
        
        protected override void Process()
        {
            Debug.Log("ProcessWait");
            var ignoredAwaitableResult = Delay();
        }
        
        private async Task Delay()
        {
            await Task.Delay((int)(Duration * 1000));
            Debug.Log("ProcessFinished");
            ProcessFinished();
        }
    }
}