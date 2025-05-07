using GraphProcessor;
using UnityEditor;
using UnityEngine.UIElements;

namespace AbilityGraph.Editor
{
    [UnityEditor.CustomEditor(typeof(Runtime.AbilityGraph), true)]
    public class AbilityGraphInspector : GraphInspector
    {
        protected override void CreateInspector()
        {
            base.CreateInspector();
            
            root.Add(new Button(() => EditorWindow.GetWindow<AbilityGraphWindow>().InitializeGraph(target as BaseGraph))
            {
                text = "Open base graph window"
            });
        }
    }
}