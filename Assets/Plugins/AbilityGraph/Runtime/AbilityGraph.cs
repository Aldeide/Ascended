using System;
using AbilityGraph.Editor;
using AbilitySystem.Runtime.Abilities;
using GraphProcessor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AbilityGraph.Runtime
{
    [Serializable]
    public class AbilityGraph : BaseGraph
    {
        public string TestName;
        
        [Button]
        public void OpenGraph()
        {
            EditorWindow.GetWindow<AbilityGraphWindow>().InitializeGraph(this as BaseGraph);
        }
    }
}