using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using Item.Runtime.Database;
using Item.Runtime.Definition;
using Item.Runtime.Interface;
using Item.Runtime.Interface.Core;
using Systems.Item.Runtime.Networking;

namespace Item.Runtime.Manager
{
    public class InventoryManager : IInventoryManager
    {
        public List<IBaseItem> Items { get; set; }
        
        private readonly IAbilitySystem _owner;
        private readonly IInventoryReplicationManager _replicationManager;
        
        public InventoryManager(IAbilitySystem owner, IInventoryReplicationManager replicationManager)
        {
            _owner = owner;
            _replicationManager = replicationManager;
            Items = new List<IBaseItem>();
        }
        
        public IAbilitySystem GetOwner()
        {
            return _owner;
        }

        public void AddItem(IBaseItem item)
        {
            if (_replicationManager.IsServer())
            {
                // We add the item on the server and notify the owner that an item was added.
                Items.Add(item);
                // TODO: Create item database entry so we can have a unique id per item.
                _replicationManager.NotifyClientAddItem(1, 1);
            }
        }

        public void AddItem(int itemId, int amount)
        {
            
        }

        public void RemoveItem(IBaseItem item)
        {
            throw new System.NotImplementedException();
        }

        public bool HasItem(IBaseItem item)
        {
            throw new System.NotImplementedException();
        }

        public bool HasItemQuantity(IBaseItem item, int quantity)
        {
            throw new System.NotImplementedException();
        }

        public bool HasItems(Dictionary<ItemDefinition, int> items)
        {
            throw new System.NotImplementedException();
        }

        public void ConsumeItems(Dictionary<ItemDefinition, int> items)
        {
            throw new System.NotImplementedException();
        }

        public ItemDefinition GetItemDefinition(string itemName)
        {
            return ItemLibrary.Instance.GetItemByName(itemName);
        }
    }
}
