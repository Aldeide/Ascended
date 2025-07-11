using AbilityGraph.Runtime.Nodes;
using GraphProcessor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace AbilityGraph.Editor
{
    public static class AbilityGraphAssetCallbacks
    {
        [MenuItem("Assets/Create/AbilityGraph", false, 10)]
        public static void CreateAbilityGraph()
        {
            var graph = ScriptableObject.CreateInstance<Runtime.AbilityGraph>();
            ProjectWindowUtil.CreateAsset(graph, "AbilityGraph.asset");
        }

        [OnOpenAsset(0)]
        public static bool OnBaseGraphOpened(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID) as BaseGraph;

            if (asset != null && AssetDatabase.GetAssetPath(asset).Contains("AbilityGraph"))
            {
                EditorWindow.GetWindow<AbilityGraphWindow>().InitializeGraph(asset as BaseGraph);
                return true;
            }
            return false;
        }
    }
}