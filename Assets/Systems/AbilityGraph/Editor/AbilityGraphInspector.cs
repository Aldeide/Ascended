using GraphProcessor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace AbilityGraph.Editor
{
    [CustomEditor(typeof(Runtime.AbilityGraph), true)]
    public class AbilityGraphInspector : OdinEditor
    {
        /*
        protected override void CreateInspector()
        {
            base.CreateInspector();
            
            root.Add(new Button(() => EditorWindow.GetWindow<AbilityGraphWindow>().InitializeGraph(target as BaseGraph))
            {
                text = "Open base graph window"
            });
        }
        */
    }
}