using System.Collections.Generic;
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
        private TreeView _treeView;
        private TextField _newTagTextField;
        private List<TreeViewItemData<TagNode>> _treeRoots;
        private int _idCounter = 0;
        private Button _deleteButton;

        [MenuItem("AbilitySystem/Gameplay Tags Editor")]
        public static void ShowWindow()
        {
            GetWindow<GameplayTagsEditor>("Gameplay Tags");
        }

        public void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Systems/GameplayTags/Editor/GameplayTagsEditor.uxml");
            visualTree.CloneTree(rootVisualElement);

            _newTagTextField = rootVisualElement.Q<TextField>("new-tag-textfield");
            _treeView = rootVisualElement.Q<TreeView>("tag-tree-view");

            rootVisualElement.Q<Button>("add-tag-button").clicked += AddTag;
            _deleteButton = rootVisualElement.Q<Button>("delete-checked-button");
            _deleteButton.clicked += DeleteCheckedTags;
            rootVisualElement.Q<Button>("save-and-generate-button").clicked += SaveAndGenerateCode;

            LoadTagData();
            RefreshTree();
        }

        private void LoadTagData()
        {
            var assetGUIDs = AssetDatabase.FindAssets("t:GameplayTagData");
            if (assetGUIDs.Length > 0)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[0]);
                _tagData = AssetDatabase.LoadAssetAtPath<GameplayTagData>(assetPath);
            }
            else
            {
                _tagData = CreateInstance<GameplayTagData>();
                AssetDatabase.CreateAsset(_tagData, "Assets/Systems/GameplayTags/GameplayTagData.asset");
                AssetDatabase.SaveAssets();
            }
            _tagData.Tags.Sort();
        }

        private void RefreshTree()
        {
            _idCounter = 0;
            _treeRoots = BuildTreeData();
            _treeView.SetRootItems(_treeRoots);

            _treeView.makeItem = () =>
            {
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.alignItems = Align.Center;

                var toggle = new Toggle();
                toggle.name = "node-toggle";
                toggle.style.marginRight = 5;

                var label = new Label();
                label.name = "node-label";

                row.Add(toggle);
                row.Add(label);
                return row;
            };

            _treeView.bindItem = (element, index) =>
            {
                var node = _treeView.GetItemDataForIndex<TagNode>(index);
                var toggle = element.Q<Toggle>("node-toggle");
                var label = element.Q<Label>("node-label");

                label.text = node.Name;
                toggle.UnregisterValueChangedCallback(OnToggleChanged);
                toggle.value = node.isChecked;
                toggle.userData = node;
                toggle.RegisterValueChangedCallback(OnToggleChanged);
            };
            
            _treeView.Rebuild();
            UpdateDeleteButtonState();
        }

        private void UpdateDeleteButtonState()
        {
            _deleteButton.SetEnabled(AnyChecked(_treeRoots));
        }

        private bool AnyChecked(IEnumerable<TreeViewItemData<TagNode>> nodes)
        {
            if (nodes == null) return false;
            foreach (var node in nodes)
            {
                if (node.data.isChecked) return true;
                if (node.children != null && AnyChecked(node.children)) return true;
            }
            return false;
        }

        private void OnToggleChanged(ChangeEvent<bool> evt)
        {
            var node = (TagNode)((VisualElement)evt.target).userData;
            node.isChecked = evt.newValue;
            UpdateDeleteButtonState();
        }

        private List<TreeViewItemData<TagNode>> BuildTreeData()
        {
            var rootNodes = new Dictionary<string, TagNode>();
            var allItems = new Dictionary<string, TreeViewItemData<TagNode>>();

            // 1. Create all nodes
            foreach (var tag in _tagData.Tags)
            {
                var parts = tag.Split('.');
                TagNode parent = null;
                string currentPath = "";

                for (int i = 0; i < parts.Length; i++)
                {
                    var partName = parts[i];
                    if (char.IsDigit(partName[0])) partName = "_" + partName;
                    
                    currentPath = string.IsNullOrEmpty(currentPath) ? partName : $"{currentPath}.{partName}";
                    string originalPath = string.Join(".", tag.Split('.').Take(i + 1));

                    if (!allItems.ContainsKey(currentPath))
                    {
                        var node = new TagNode { Name = partName, FullPath = originalPath };
                        var itemData = new TreeViewItemData<TagNode>(_idCounter++, node, new List<TreeViewItemData<TagNode>>());
                        allItems[currentPath] = itemData;

                        if (parent == null)
                        {
                            rootNodes[currentPath] = node;
                        }
                        else
                        {
                            // Add to parent's children
                            var parentItem = allItems[string.Join(".", currentPath.Split('.').Take(i))];
                            ((List<TreeViewItemData<TagNode>>)parentItem.children).Add(itemData);
                        }
                    }
                    parent = allItems[currentPath].data;
                }
            }

            // Return only root items
            return allItems.Values.Where(v => !v.data.FullPath.Contains(".")).ToList();
        }

        private void AddTag()
        {
            var newTag = _newTagTextField.value;
            if (string.IsNullOrEmpty(newTag) || _tagData.Tags.Contains(newTag)) return;
            _tagData.Tags.Add(newTag);
            _tagData.Tags.Sort();
            RefreshTree();
            _newTagTextField.value = "";
        }

        private void DeleteCheckedTags()
        {
            var tagsToDelete = new HashSet<string>();
            
            void CollectChecked(IEnumerable<TreeViewItemData<TagNode>> items)
            {
                foreach (var item in items)
                {
                    if (item.data.isChecked)
                    {
                        tagsToDelete.Add(item.data.FullPath);
                    }
                    else if (item.children != null)
                    {
                        CollectChecked(item.children);
                    }
                }
            }

            CollectChecked(_treeRoots);

            if (tagsToDelete.Count == 0) return;

            // Remove tags and all children (starts with prefix)
            _tagData.Tags.RemoveAll(t => tagsToDelete.Any(prefix => t == prefix || t.StartsWith(prefix + ".")));

            EditorUtility.SetDirty(_tagData);
            AssetDatabase.SaveAssets();
            
            RefreshTree();
            GenerateCode();
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
            var path = "Assets/Systems/GameplayTags/Generated/GameplayTags.cs";
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("// -- AUTO-GENERATED FILE --");
                writer.WriteLine("using GameplayTags.Runtime;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.Linq;");
                writer.WriteLine("");
                writer.WriteLine("namespace GameplayTags.Generated");
                writer.WriteLine("{");
                writer.WriteLine("    public static class TagLibrary");
                writer.WriteLine("    {");

                var roots = BuildTreeData();
                WriteNodesToCode(writer, roots, 2);

                writer.WriteLine("");
                writer.WriteLine("        private static readonly List<Tag> AllTags = new List<Tag>");
                writer.WriteLine("        {");
                foreach (var tag in _tagData.Tags)
                {
                    writer.WriteLine($"            new Tag(\"{tag}\"),");
                }
                writer.WriteLine("        };");
                writer.WriteLine("");
                writer.WriteLine("        public static IReadOnlyList<Tag> GetAllTags() => AllTags;");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
            AssetDatabase.Refresh();
        }

        private void WriteNodesToCode(StreamWriter writer, IEnumerable<TreeViewItemData<TagNode>> items, int indentLevel)
        {
            var indent = new string(' ', indentLevel * 4);
            var staticModifier = indentLevel == 2 ? "static " : "";
            
            foreach (var item in items.OrderBy(x => x.data.Name))
            {
                var className = $"__{item.data.FullPath.Replace(".", "_")}_Group";
                
                if (item.children != null && item.children.Any())
                {
                    writer.WriteLine($"{indent}public class {className}");
                    writer.WriteLine($"{indent}{{");
                    writer.WriteLine($"{indent}    public static implicit operator Tag({className} _) => new Tag(\"{item.data.FullPath}\");");
                    WriteNodesToCode(writer, item.children, indentLevel + 1);
                    writer.WriteLine($"{indent}}}");
                    
                    writer.WriteLine($"{indent}public {staticModifier}readonly {className} {item.data.Name} = new {className}();");
                }
                else
                {
                    writer.WriteLine($"{indent}public {staticModifier}readonly Tag {item.data.Name} = new Tag(\"{item.data.FullPath}\");");
                }
            }
        }
        private class TagNode
        {
            public string Name;
            public string FullPath;
            public bool isChecked;
        }
    }
}