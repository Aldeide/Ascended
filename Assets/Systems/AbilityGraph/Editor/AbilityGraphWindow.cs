using GraphProcessor;
using UnityEditor;
using UnityEngine;

namespace AbilityGraph.Editor
{
    public class AbilityGraphWindow : BaseGraphWindow
    {
        private BaseGraph _tmpGraph;
        // The window is opened via OnOpenAsset in AbilityGraphAssetCallbacks

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
