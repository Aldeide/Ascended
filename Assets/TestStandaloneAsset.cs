using UnityEditor;
using UnityEngine;
using Systems.AbilitySystemGraph.Editor;
using Systems.AbilitySystemGraph.Runtime;
using System.Reflection;

public static class TestStandaloneAsset
{
    [MenuItem("Tools/Test Create Standalone Graph Asset")]
    public static void Create()
    {
        var assembly = typeof(Unity.GraphToolkit.Editor.Graph).Assembly;
        var modelType = assembly.GetType("Unity.GraphToolkit.Editor.Model");
        var graphModelImpType = assembly.GetType("Unity.GraphToolkit.Editor.Implementation.GraphModelImp");
        
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Is Model a ScriptableObject? " + modelType.IsSubclassOf(typeof(ScriptableObject)));
        sb.AppendLine("Is GraphModelImp a ScriptableObject? " + graphModelImpType.IsSubclassOf(typeof(ScriptableObject)));
        
        // Also scan for OnOpenAsset as requested before
        sb.AppendLine("\n=== Scanning for OnOpenAsset ===");
        foreach (var type in assembly.GetTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                var attrs = method.GetCustomAttributes(typeof(UnityEditor.Callbacks.OnOpenAssetAttribute), true);
                if (attrs.Length > 0)
                {
                    sb.AppendLine("Type: " + type.FullName + " Method: " + method.Name);
                }
            }
        }
        
        System.IO.File.WriteAllText("GraphObjectDump.txt", sb.ToString());
        Debug.Log("Dumped Model details to GraphObjectDump.txt");
    }
}
