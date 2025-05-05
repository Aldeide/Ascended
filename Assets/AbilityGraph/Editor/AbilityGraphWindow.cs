using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityGraphWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/AbilityGraphWindow")]
    public static void ShowExample()
    {
        AbilityGraphWindow wnd = GetWindow<AbilityGraphWindow>();
        wnd.titleContent = new GUIContent("AbilityGraphWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
    }
}
