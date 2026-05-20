using System;
using System.Collections.Generic;
using System.Linq;
using AbilityGraph.Runtime.Nodes;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime
{
    /// <summary>
    /// Executes an ability graph starting from a given start node.
    /// Pre-allocates its working collections to avoid per-activation GC pressure.
    /// </summary>
    public class GraphRunner
    {
        private readonly AbilityStartNode _startNode;
        private readonly GraphContext _context;

        // Pre-allocated to avoid per-Run() heap allocations.
        private readonly Stack<BaseNode> _nodesToExecute = new Stack<BaseNode>();
        private readonly HashSet<BaseNode> _nodeDependenciesGathered = new HashSet<BaseNode>();

        private bool _cancelled;

        public GraphRunner(AbilityStartNode startNode, GraphContext context)
        {
            _startNode = startNode;
            _context = context;
        }

        /// <summary>
        /// Cancel this runner. Any pending WaitableNode callbacks will be ignored.
        /// </summary>
        public void Cancel()
        {
            _cancelled = true;
        }

        /// <summary>
        /// Runs the graph synchronously from the start node.
        /// WaitableNodes suspend the synchronous chain and resume asynchronously via callbacks.
        /// </summary>
        public void Run()
        {
            _cancelled = false;
            RunFrom(_startNode);
        }

        private void RunFrom(BaseNode startNode)
        {
            _nodesToExecute.Clear();
            _nodeDependenciesGathered.Clear();

            _nodesToExecute.Push(startNode);
            var enumerator = RunGraph(_nodesToExecute, _nodeDependenciesGathered);

            while (enumerator.MoveNext())
            {
                if (_cancelled || !_context.Ability.IsActive) return;
            }
        }

        private IEnumerator<BaseNode> RunGraph(
            Stack<BaseNode> nodesToExecute,
            HashSet<BaseNode> nodeDependenciesGathered)
        {
            while (nodesToExecute.Count > 0)
            {
                if (_cancelled || !_context.Ability.IsActive) yield break;

                var node = nodesToExecute.Pop();

                if (node is IExecutableNode && !nodeDependenciesGathered.Contains(node))
                {
                    // First visit: gather data-node dependencies before executing.
                    nodesToExecute.Push(node);
                    nodeDependenciesGathered.Add(node);

                    foreach (var dep in GatherNonConditionalDependencies(node))
                        nodesToExecute.Push(dep);
                }
                else
                {
                    // Second visit (or plain data node): execute.
                    nodeDependenciesGathered.Remove(node);

                    if (!ExecuteNode(node)) yield break;
                    yield return node;

                    if (node is IExecutableNode execNode)
                    {
                        switch (node)
                        {
                            case WaitableNode waitableNode:
                                // Push the "immediate" successor chain (nodes that run in parallel while waiting).
                                foreach (var n in waitableNode.GetExecutedNodes())
                                    nodesToExecute.Push(n);

                                // When the wait completes, resume a new sub-chain from the "Execute After" port.
                                waitableNode.onProcessFinished += OnWaitableFinished;
                                break;

                            default:
                                foreach (var n in execNode.GetExecutedNodes())
                                    nodesToExecute.Push(n);
                                break;
                        }
                    }
                }
            }
        }

        private void OnWaitableFinished(WaitableNode waitedNode)
        {
            waitedNode.onProcessFinished -= OnWaitableFinished;

            if (_cancelled || !_context.Ability.IsActive) return;

            // Resume execution from the "Execute After" port in a fresh sub-graph pass.
            var resumeNodes = new Stack<BaseNode>();
            foreach (var n in waitedNode.GetExecuteAfterNodes())
                resumeNodes.Push(n);

            if (resumeNodes.Count == 0) return;

            // Use a dedicated enumerator so the resumed chain has its own dependency set.
            var depSet = new HashSet<BaseNode>();
            var enumerator = RunGraph(resumeNodes, depSet);
            while (enumerator.MoveNext())
            {
                if (_cancelled || !_context.Ability.IsActive) return;
            }
        }

        /// <summary>
        /// Executes a single node, catching any exceptions to prevent the ability
        /// from being stuck in IsActive = true on graph errors.
        /// </summary>
        private bool ExecuteNode(BaseNode node)
        {
            try
            {
                node.OnProcess();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GraphRunner] Exception in node '{node.name}': {ex}");
                _context.Ability.EndAbility();
                return false;
            }
        }

        /// <summary>
        /// Traverses input connections to collect all non-executable (data) dependency nodes
        /// that must be evaluated before <paramref name="node"/> can execute.
        /// </summary>
        private static IEnumerable<BaseNode> GatherNonConditionalDependencies(BaseNode node)
        {
            var stack = new Stack<BaseNode>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                foreach (var dep in current.GetInputNodes())
                {
                    if (dep is IExecutableNode) continue; // Stop at execution boundaries.
                    stack.Push(dep);
                    yield return dep;
                }
            }
        }
    }
}