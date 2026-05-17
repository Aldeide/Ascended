using UnityEditor;
using UnityEngine;
using Unity.GraphToolkit.Editor;
using Systems.AbilitySystemGraph.Runtime;
using Systems.AbilitySystemGraph.Editor.Nodes;
using System.Linq;

namespace Systems.AbilitySystemGraph.Editor
{
    /// <summary>
    /// Custom Inspector for GraphAbilityDefinition that provides the 
    /// "All-in-One" workflow for designers.
    /// </summary>
    [CustomEditor(typeof(GraphAbilityDefinition))]
    public class GraphAbilityDefinitionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the base ScriptableObject fields (Description, Icon, etc.)
            base.OnInspectorGUI();

            var definition = (GraphAbilityDefinition)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Graph Editor", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Open Ability Graph Editor", GUILayout.Height(40)))
            {
                OpenGraphEditor(definition);
            }

            if (GUILayout.Button("Force Re-Bake Bytecode"))
            {
                BakeGraph(definition);
            }
        }

        private void OpenGraphEditor(GraphAbilityDefinition definition)
        {
            AbilitySystemGraphWindow.Open(definition);
        }

        private void BakeGraph(GraphAbilityDefinition definition)
        {
            AbilityGraph graph = new AbilityGraph();
            if (!string.IsNullOrEmpty(definition.SerializedGraphData))
            {
                EditorJsonUtility.FromJsonOverwrite(definition.SerializedGraphData, graph);
                AbilityGraphBaker.BakeGraph(graph, definition);
                EditorUtility.SetDirty(definition);
            }
        }
    }
}
