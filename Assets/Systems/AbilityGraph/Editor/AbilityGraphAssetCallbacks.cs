using AbilityGraph.Runtime.Nodes;
using GraphProcessor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace AbilityGraph.Editor
{
    public static class AbilityGraphAssetCallbacks
    {
        [OnOpenAsset(0)]
        public static bool OnBaseGraphOpened(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID);

            if (asset is AbilitySystem.Runtime.Abilities.AbilityDefinition abilityDef)
            {
                EditorWindow.GetWindow<AbilityGraphWindow>().InitializeGraph(abilityDef);
                return true;
            }
            return false;
        }
    }
}