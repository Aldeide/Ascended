using System;
using UnityEditor;
using UnityEngine;
using Unity.GraphToolkit.Editor;

namespace Systems.AbilitySystemGraph.Editor
{
    /// <summary>
    /// The Authoring Graph for the Ability System.
    /// In GTK 0.4, this class defines the custom graph format and its extension.
    /// It is NOT a ScriptableObject; GTK handles its own asset persistence.
    /// </summary>
    [Serializable]
    [Graph(AssetExtension)]
    public class AbilityGraph : Graph
    {
        public const string AssetExtension = "abilitygraph";
    }
}
