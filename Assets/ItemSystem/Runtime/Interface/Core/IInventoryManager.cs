using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using ItemSystem.Runtime.Definition;

namespace ItemSystem.Runtime.Interface.Core
{
    public interface IInventoryManager
    {
        public List<IBaseItem> Items { get; set; }

        public IAbilitySystem GetOwner();
        
        public void AddItem(IBaseItem item);
        public void RemoveItem(IBaseItem item);
        public bool HasItem(IBaseItem item);
        public bool HasItemQuantity(IBaseItem item, int quantity);
        public bool HasItems(Dictionary<ItemDefinition, int> items);
        public void ConsumeItems(Dictionary<ItemDefinition, int> items);
    }
}