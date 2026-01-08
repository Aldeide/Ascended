using Item.Runtime.Interface;
using UnityEngine.Localization;

namespace Systems.Item.Tests.Utilities
{
    public static class TestItems
    {
        public static Item BasicItem()
        {
            return new Item
            {
                Name = "BasicItem"
            };
        }
    }
    public class Item : IBaseItem {
        public string Name { get; set; }
        public LocalizedString DisplayName { get; set; }
        public LocalizedString Description { get; set; }
    }
}