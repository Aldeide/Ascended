using System.Collections.Generic;
using System.Linq;
using AbilityGraph.Runtime.Nodes;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime
{
    public class GraphRunner
    {
        private AbilityStartNode _startNode;
        
        public GraphRunner(AbilityStartNode node)
        {
            _startNode = node;
        }

        public void Run()
        {
            IEnumerator<BaseNode> enumerator;
            Stack<BaseNode> nodesToExecute = new Stack<BaseNode>();
            nodesToExecute.Push(_startNode);
            enumerator = RunGraph(nodesToExecute);
            while (enumerator.MoveNext());
        }
        
        private IEnumerator<BaseNode> RunGraph(Stack<BaseNode> nodesToExecute)
        {
            HashSet<BaseNode> nodeDependenciesGathered = new HashSet<BaseNode>();
            HashSet<BaseNode> skipConditionalHandling  = new HashSet<BaseNode>();

            while (nodesToExecute.Count > 0)
            {
                var node = nodesToExecute.Pop();
                if(node is IExecutableNode && !skipConditionalHandling.Contains(node))
				{
					// Gather non-conditional deps: TODO, move to the cache:
					if(nodeDependenciesGathered.Contains(node))
					{
						// Execute the conditional node:
						node.OnProcess();
						yield return node;
	
						// And select the next nodes to execute:
						switch(node)
						{
							/*
							// special code path for the loop node as it will execute multiple times the same nodes
							case ForLoopNode forLoopNode:
								forLoopNode.index = forLoopNode.start - 1; // Initialize the start index
								foreach(var n in forLoopNode.GetExecutedNodesLoopCompleted())
									nodeToExecute.Push(n);
								for(int i = forLoopNode.start; i < forLoopNode.end; i++)
								{
									foreach(var n in forLoopNode.GetExecutedNodesLoopBody())
										nodeToExecute.Push(n);
	
									nodeToExecute.Push(node); // Increment the counter
								}
	
								skipConditionalHandling.Add(node);
								break;
								**/
							// another special case for waitable nodes, like "wait for a coroutine", wait x seconds", etc.
							case WaitableNode waitableNode:
								foreach(var n in waitableNode.GetExecutedNodes())
									nodesToExecute.Push(n);
	
								waitableNode.onProcessFinished += (waitedNode) =>
								{
									Stack<BaseNode> waitedNodes = new Stack<BaseNode>();
									foreach(var n in waitedNode.GetExecuteAfterNodes())
										waitedNodes.Push(n);
									WaitedRun(waitedNodes);
									waitableNode.onProcessFinished = null;
								};
								break;
							case IExecutableNode cNode:
								foreach(var n in cNode.GetExecutedNodes())
									nodesToExecute.Push(n);
								break;
							default:
								Debug.LogError($"Conditional node {node} not handled");
								break;
						}
	
						nodeDependenciesGathered.Remove(node);
					}
					else
					{
						nodesToExecute.Push(node);
						nodeDependenciesGathered.Add(node);
						foreach(var nonConditionalNode in GatherNonConditionalDependencies(node))
						{
							nodesToExecute.Push(nonConditionalNode);
						}
					}
				}
				else
				{
					node.OnProcess();
					yield return node;
				}
            }

        }
        
        private void WaitedRun(Stack<BaseNode> nodesToRun)
        {
	        // Execute the waitable node:
	        var enumerator = RunGraph(nodesToRun);
	        while(enumerator.MoveNext()) ;
        }

        IEnumerable<BaseNode> GatherNonConditionalDependencies(BaseNode node)
        {
	        Stack<BaseNode> dependencies = new Stack<BaseNode>();

	        dependencies.Push(node);
        
	        while (dependencies.Count > 0)
	        {
		        var dependency = dependencies.Pop();

		        foreach (var d in dependency.GetInputNodes().Where(n => !(n is IExecutableNode)))
			        dependencies.Push(d);

		        if (dependency != node)
			        yield return dependency;
	        }
        }
    }
}