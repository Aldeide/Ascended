using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class AbilitySystemDebugWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private GameObject _inspectedObject;
    private IAbilitySystem _inspectedAsc;
    private VisualElement _label;
    private VisualElement _text;
    
    [MenuItem("Window/UI Toolkit/AbilitySystemDebugWindow")]
    public static void ShowExample()
    {
        AbilitySystemDebugWindow wnd = GetWindow<AbilitySystemDebugWindow>();
        wnd.titleContent = new GUIContent("AbilitySystemDebugWindow");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        if (_label == null)
        {
            _label = new Label("Placeholder");
        }

        if (_text == null)
        {
            _text = new Label("Placeholder");
        }
        root.Add(_label);
        root.Add(_text);
    }

    private void OnSelectionChange()
    {
        if (Selection.objects.Length == 1)
        {
            _inspectedObject = (GameObject)Selection.objects[0];
            _label = new Label(_inspectedObject.gameObject.name);
            var asc = _inspectedObject.GetComponent<AbilitySystemComponent>();
            if (asc)
            {
                _inspectedAsc = asc.AbilitySystem;
                _text = new Label(DisplayData());
            }
            rootVisualElement.Clear();
            CreateGUI();
        }
    }

    private void Update()
    {
        if (_inspectedAsc == null) return;
        _text = new Label(DisplayData());
        rootVisualElement.Clear();
        CreateGUI();
    }
    
    private string DisplayData()
    {
        var output = _inspectedAsc.AttributeSetManager.DebugString() + "\n\n";
        output += _inspectedAsc.EffectManager.DebugString() + "\n\n";
        output += _inspectedAsc.AbilityManager.DebugString() + "\n\n";
        output += _inspectedAsc.TagManager.DebugString() + "\n\n";
        return output;
    }
}
