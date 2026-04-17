using Item.Runtime.Definition;
using Item.Runtime.Interface;
using UnityEngine;
using UnityEngine.Localization;

namespace Systems.Item.Tests.Utilities
{
    public static class TestItems
    {
        public static TestItem BasicItem()
        {
            var itemDefinition = ScriptableObject.CreateInstance<EquipmentDefinition>();
            itemDefinition.Name = "TestItem";
            return new TestItem(itemDefinition);
        }
    }
    public class TestItem : IBaseItem
    {
        public string Name { get; set; }
        public LocalizedString DisplayName { get; set; }
        public LocalizedString Description { get; set; }
        public UnityEngine.Sprite Icon { get; set; }
        
        public TestItem(ItemDefinition definition)
        {
            Name = definition.Name;
            DisplayName = definition.DisplayName;
            Description = definition.Description;
            Icon = definition.Icon;
        }
    }
}