using UnityEditor;
using UnityEngine;
using Systems.AbilitySystemGraph.Runtime;
using System.Linq;

namespace Systems.AbilitySystemGraph.Editor
{
    /// <summary>
    /// Translates the visual GTK AbilityGraph into a flat array of Instructions
    /// for the Burst-compiled Graph VM.
    /// </summary>
    public static class AbilityGraphBaker
    {
        public static void BakeGraph(AbilityGraph editorGraph, GraphAbilityDefinition runtimeGraph)
        {
            if (editorGraph == null || runtimeGraph == null) return;

            runtimeGraph.CompiledInstructions.Clear();
            runtimeGraph.FloatRegisterCount = 0;
            
            // 1. Locate the locked entry point
            var activateNode = editorGraph.GetNodes().OfType<Nodes.ActivateAbilityNode>().FirstOrDefault();
            if (activateNode == null)
            {
                Debug.LogError("Bake Failed: Graph is missing the required ActivateAbilityNode.");
                return;
            }

            // 2. Traversal
            var visitedNodes = new System.Collections.Generic.HashSet<Unity.GraphToolkit.Editor.Node>();
            TraverseExecutionFlow(activateNode, editorGraph, runtimeGraph, visitedNodes);
            
            EditorUtility.SetDirty((UnityEngine.Object)runtimeGraph);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"Baked Ability Graph for {runtimeGraph.name}. Generated {runtimeGraph.CompiledInstructions.Count} instructions.");
        }

        private static void TraverseExecutionFlow(Unity.GraphToolkit.Editor.Node currentNode, AbilityGraph graph, GraphAbilityDefinition runtimeGraph, System.Collections.Generic.HashSet<Unity.GraphToolkit.Editor.Node> visited)
        {
            if (currentNode == null || visited.Contains(currentNode)) return;
            visited.Add(currentNode);

            // Evaluate the node and push its Instruction to the flat array
            if (currentNode is Nodes.AbilityNode abilityNode)
            {
                Systems.AbilitySystemGraph.VM.OpCode op = abilityNode.GetOpCode();
                
                if (op != Systems.AbilitySystemGraph.VM.OpCode.Nop)
                {
                    runtimeGraph.CompiledInstructions.Add(new Systems.AbilitySystemGraph.VM.Instruction
                    {
                        OpCode = op,
                    });
                }
            }

            // Recursively follow the execution flow
            var nextNode = GetNextExecutionNode(currentNode, graph);
            if (nextNode != null)
            {
                TraverseExecutionFlow(nextNode, graph, runtimeGraph, visited);
            }
        }

        private static Unity.GraphToolkit.Editor.Node GetNextExecutionNode(Unity.GraphToolkit.Editor.Node fromNode, AbilityGraph graph)
        {
            // TODO: Use actual GTK Edge tracing logic for GTK 0.4.
            return null;
        }
    }
}
