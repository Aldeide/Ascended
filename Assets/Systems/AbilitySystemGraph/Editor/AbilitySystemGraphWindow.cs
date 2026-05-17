using UnityEditor;
using UnityEngine;
using Systems.AbilitySystemGraph.Runtime;
using System.Reflection;

namespace Systems.AbilitySystemGraph.Editor
{
    /// <summary>
    /// Utility class to open the GTK Editor by utilizing Unity's native Asset management.
    /// Instead of hacking the window, we create the GTK GraphObject as a hidden sub-asset 
    /// of our GraphAbilityDefinition and let Unity's native double-click behavior handle the rest.
    /// </summary>
    public static class AbilitySystemGraphWindow
    {
        public static void Open(GraphAbilityDefinition definition)
        {
            var assembly = typeof(Unity.GraphToolkit.Editor.Graph).Assembly;
            var concreteGraphObjectType = assembly.GetType("Unity.GraphToolkit.Editor.Implementation.GraphObjectImp");

            if (concreteGraphObjectType == null)
            {
                Debug.LogError("Could not find concrete GraphObjectImp type.");
                return;
            }

            // 1. Look for an existing GraphObject sub-asset
            Object existingGraphObject = null;
            var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(definition));
            foreach (var asset in assets)
            {
                if (asset != null && asset.GetType() == concreteGraphObjectType)
                {
                    existingGraphObject = asset;
                    break;
                }
            }

            // 2. If it doesn't exist, create it as a sub-asset
            if (existingGraphObject == null)
            {
                var newGraphObject = ScriptableObject.CreateInstance(concreteGraphObjectType);
                newGraphObject.name = "AbilityGraphData";
                
                // Hide it in the project window so the user only sees the main Ability Definition
                newGraphObject.hideFlags = HideFlags.HideInHierarchy;

                // Create the underlying graph data structure
                var graph = new AbilityGraph();
                if (!string.IsNullOrEmpty(definition.SerializedGraphData))
                {
                    EditorJsonUtility.FromJsonOverwrite(definition.SerializedGraphData, graph);
                }

                // Assign graph to GraphObject
                var graphObjectType = assembly.GetType("Unity.GraphToolkit.Editor.GraphObject");
                var graphProp = graphObjectType.GetProperty("graph", BindingFlags.Public | BindingFlags.Instance);
                if (graphProp != null) graphProp.SetValue(newGraphObject, graph);

                // Add as sub-asset
                AssetDatabase.AddObjectToAsset(newGraphObject, definition);
                AssetDatabase.SaveAssets();
                
                existingGraphObject = newGraphObject;
            }

            // 3. Open it natively! GTK's own OnOpenAsset handler will intercept this and open its window perfectly.
            AssetDatabase.OpenAsset(existingGraphObject);
        }
    }
}
