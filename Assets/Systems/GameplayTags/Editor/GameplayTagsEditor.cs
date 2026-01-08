using System.IO;
using System.Linq;
using GameplayTags.Runtime;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameplayTags.Editor
{
    public class GameplayTagsEditor : EditorWindow
    {
        private GameplayTagData _tagData;
        private ListView _listView;
        private TextField _newTagTextField;

        [MenuItem("AbilitySystem/Gameplay Tags Editor")]
        public static void ShowWindow()
        {
            GetWindow<GameplayTagsEditor>("Gameplay Tags");
        }

        public void CreateGUI()
        {
            // Load the UXML file
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Plugins/GameplayTags/Editor/GameplayTagsEditor.uxml");
            visualTree.CloneTree(rootVisualElement);

            // Find the UI elements
            _newTagTextField = rootVisualElement.Q<TextField>("new-tag-textfield");
            _listView = rootVisualElement.Q<ListView>("tag-list-view");

            // Register button callbacks
            rootVisualElement.Q<Button>("add-tag-button").clicked += AddTag;
            rootVisualElement.Q<Button>("delete-tag-button").clicked += DeleteSelectedTag;
            rootVisualElement.Q<Button>("save-and-generate-button").clicked += SaveAndGenerateCode;

            // Load the tag data
            LoadTagData();
            BindListView();
        }

        private void LoadTagData()
        {
            // Find the ScriptableObject asset
            var assetGUIDs = AssetDatabase.FindAssets("t:GameplayTagData");
            if (assetGUIDs.Length > 0)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[0]);
                _tagData = AssetDatabase.LoadAssetAtPath<GameplayTagData>(assetPath);
            }
            else
            {
                // If no asset exists, create one
                _tagData = CreateInstance<GameplayTagData>();
                AssetDatabase.CreateAsset(_tagData, "Assets/Plugins/GameplayTags/GameplayTagData.asset");
                AssetDatabase.SaveAssets();
            }

            // Sort the tags alphabetically
            _tagData.Tags.Sort();
        }

        private void BindListView()
        {
            _listView.makeItem = () => new Label();
            _listView.bindItem = (element, i) => ((Label)element).text = _tagData.Tags[i];
            _listView.itemsSource = _tagData.Tags;
        }

        private void AddTag()
        {
            var newTag = _newTagTextField.value;
            if (string.IsNullOrEmpty(newTag) || _tagData.Tags.Contains(newTag)) return;
            _tagData.Tags.Add(newTag);
            // Keep the list sorted alphabetically
            _tagData.Tags.Sort();
            _listView.Rebuild();
            _newTagTextField.value = "";
        }

        private void DeleteSelectedTag()
        {
            var selectedIndex = _listView.selectedIndex;
            if (selectedIndex < 0 || selectedIndex >= _tagData.Tags.Count) return;
            _tagData.Tags.RemoveAt(selectedIndex);
            _listView.Rebuild();
        }


        private void SaveAndGenerateCode()
        {
            if (!_tagData) return;

            EditorUtility.SetDirty(_tagData);
            AssetDatabase.SaveAssets();

            GenerateCode();

            EditorUtility.DisplayDialog("Success", "Tags saved and code generated!", "OK");
        }

        private void GenerateCode()
        {
            var path = "Assets/Plugins/GameplayTags/Generated/GameplayTags.cs";

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("// -- AUTO-GENERATED FILE --");
                writer.WriteLine("using GameplayTags.Runtime;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("");
                writer.WriteLine("namespace GameplayTags.Generated");
                writer.WriteLine("{");
                writer.WriteLine("    public static class TagLibrary");
                writer.WriteLine("    {");

                var sanitizedTags = _tagData.Tags
                    .Select(tag => new { Original = tag, Sanitized = tag.Replace(".", "") })
                    .ToList();

                foreach (var t in sanitizedTags)
                {
                    writer.WriteLine($"        public static readonly Tag {t.Sanitized} = new Tag(\"{t.Original}\");");
                }

                writer.WriteLine("");
                writer.WriteLine("        private static readonly List<Tag> AllTags = new List<Tag>");
                writer.WriteLine("        {");

                foreach (var t in sanitizedTags)
                {
                    writer.WriteLine($"            {t.Sanitized},");
                }

                writer.WriteLine("        };");
                writer.WriteLine("");
                writer.WriteLine("        public static IReadOnlyList<Tag> GetAllTags()");
                writer.WriteLine("        {");
                writer.WriteLine("            return AllTags;");
                writer.WriteLine("        }");

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }
    }
}