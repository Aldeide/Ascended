using GraphProcessor;
using UnityEditor;
using UnityEngine;

namespace AbilityGraph.Editor
{
    public class AbilityGraphWindow : BaseGraphWindow
    {
        private BaseGraph _tmpGraph;
        // Add the window in the editor menu
        [MenuItem("Window/01_DefaultGraph")]
        public static AbilityGraphWindow Open()
        {
            var graphWindow = EditorWindow.CreateWindow<AbilityGraphWindow>();
            graphWindow._tmpGraph = CreateInstance<Runtime.AbilityGraph>();
            graphWindow._tmpGraph.hideFlags = HideFlags.HideAndDontSave;
            graphWindow.InitializeGraph(graphWindow._tmpGraph);
            graphWindow.Show();
            
            return graphWindow;
        }

        protected override void InitializeWindow(BaseGraph baseGraph)
        {
            // Set the window title
            titleContent = new GUIContent("Ability Graph Editor");

            // Here you can use the default BaseGraphView or a custom one (see section below)
            if (graphView == null)
            {
                graphView = new AbilityGraphView(this);
                graphView.Add(new MiniMapView(graphView));
            }
                
                
            rootView.Add(graphView);
        }

        protected override void InitializeGraphView(BaseGraphView view)
        {
            view.OpenPinned<ExposedParameterView>();
        }
    }
}
