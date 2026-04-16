using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilitySystem.Editor
{
    public class AttributeSetEditorWindow : EditorWindow
    {
        private AttributeSetData _data;
        private ListView _setListView;
        private ListView _attributeListView;
        private TextField _newSetTextField;
        private TextField _newAttributeTextField;
        private Label _selectedSetLabel;

        private AttributeSetDefinition _selectedSet;

        [MenuItem("AbilitySystem/Attribute Set Editor")]
        public static void ShowWindow()
        {
            GetWindow<AttributeSetEditorWindow>("Attribute Set Editor");
        }

        public void CreateGUI()
        {
            // Load UXML
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Systems/AbilitySystem/Editor/UI/AttributeSetEditor.uxml");
            if (uxml == null) return;
            uxml.CloneTree(rootVisualElement);

            // Load USS
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Systems/AbilitySystem/Editor/UI/AttributeSetEditor.uss");
            if (uss != null) rootVisualElement.styleSheets.Add(uss);

            // Find Elements
            _newSetTextField = rootVisualElement.Q<TextField>("new-set-textfield");
            _setListView = rootVisualElement.Q<ListView>("set-list-view");
            _newAttributeTextField = rootVisualElement.Q<TextField>("new-attribute-textfield");
            _attributeListView = rootVisualElement.Q<ListView>("attribute-list-view");
            _selectedSetLabel = rootVisualElement.Q<Label>("selected-set-label");

            // Callbacks
            rootVisualElement.Q<Button>("add-set-button").clicked += AddSet;
            rootVisualElement.Q<Button>("delete-set-button").clicked += DeleteSelectedSet;
            rootVisualElement.Q<Button>("add-attribute-button").clicked += AddAttribute;
            rootVisualElement.Q<Button>("delete-attribute-button").clicked += DeleteSelectedAttribute;
            rootVisualElement.Q<Button>("save-and-generate-button").clicked += SaveAndGenerate;

            _setListView.selectionChanged += OnSetSelectionChanged;

            // Load Data
            LoadData();
            SetupSetListView();
            SetupAttributeListView();
        }

        private void LoadData()
        {
            var guids = AssetDatabase.FindAssets("t:AttributeSetData");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _data = AssetDatabase.LoadAssetAtPath<AttributeSetData>(path);
            }
            else
            {
                _data = CreateInstance<AttributeSetData>();
                AssetDatabase.CreateAsset(_data, "Assets/Systems/AbilitySystem/Editor/AttributeSetData.asset");
                AssetDatabase.SaveAssets();
            }
        }

        private void SetupSetListView()
        {
            _setListView.makeItem = () => new Label();
            _setListView.bindItem = (element, i) => ((Label)element).text = _data.AttributeSets[i].SetName;
            _setListView.itemsSource = _data.AttributeSets;
        }

        private void SetupAttributeListView()
        {
            _attributeListView.makeItem = () => new Label();
            _attributeListView.bindItem = (element, i) =>
            {
                if (_selectedSet != null && i < _selectedSet.Attributes.Count)
                {
                    ((Label)element).text = _selectedSet.Attributes[i];
                }
            };
        }

        private void OnSetSelectionChanged(IEnumerable<object> selectedItems)
        {
            _selectedSet = selectedItems.FirstOrDefault() as AttributeSetDefinition;
            if (_selectedSet != null)
            {
                _selectedSetLabel.text = $"Attributes for: {_selectedSet.SetName}";
                _attributeListView.itemsSource = _selectedSet.Attributes;
            }
            else
            {
                _selectedSetLabel.text = "Select a set...";
                _attributeListView.itemsSource = null;
            }
            _attributeListView.Rebuild();
        }

        private void AddSet()
        {
            var name = _newSetTextField.value;
            if (string.IsNullOrEmpty(name) || _data.AttributeSets.Any(s => s.SetName == name)) return;

            _data.AttributeSets.Add(new AttributeSetDefinition { SetName = name });
            _setListView.Rebuild();
            _newSetTextField.value = "";
            EditorUtility.SetDirty(_data);
        }

        private void DeleteSelectedSet()
        {
            if (_selectedSet == null) return;
            _data.AttributeSets.Remove(_selectedSet);
            _selectedSet = null;
            _setListView.Rebuild();
            _attributeListView.itemsSource = null;
            _attributeListView.Rebuild();
            _selectedSetLabel.text = "Select a set...";
            EditorUtility.SetDirty(_data);
        }

        private void AddAttribute()
        {
            if (_selectedSet == null) return;
            var name = _newAttributeTextField.value;
            if (string.IsNullOrEmpty(name) || _selectedSet.Attributes.Contains(name)) return;

            _selectedSet.Attributes.Add(name);
            _attributeListView.Rebuild();
            _newAttributeTextField.value = "";
            EditorUtility.SetDirty(_data);
        }

        private void DeleteSelectedAttribute()
        {
            if (_selectedSet == null) return;
            var selectedIndex = _attributeListView.selectedIndex;
            if (selectedIndex < 0) return;

            _selectedSet.Attributes.RemoveAt(selectedIndex);
            _attributeListView.Rebuild();
            EditorUtility.SetDirty(_data);
        }

        private void SaveAndGenerate()
        {
            if (_data == null) return;
            EditorUtility.SetDirty(_data);
            AssetDatabase.SaveAssets();
            
            AttributeSetCodeGenerator.Generate(_data);
            
            EditorUtility.DisplayDialog("Attribute Set Editor", "Code Generated Successfully!", "OK");
        }
    }
}
