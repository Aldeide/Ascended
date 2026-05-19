using System;
using System.Collections.Generic;
using System.Linq;
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
        public event Action OnInventoryChanged;
        
        private readonly IAbilitySystem _owner;
        private readonly IItemReplicationManager _replicationManager;

        public InventoryManager(IAbilitySystem owner, IItemReplicationManager replicationManager)
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
            Items.Add(item);
            if (_replicationManager != null && _replicationManager.IsServer())
            {
                // TODO: Create item database entry so we can have a unique id per item.
                _replicationManager.NotifyClientItemAdded(item.Name, 1);
            }
            OnInventoryChanged?.Invoke();
        }

        public void AddItem(string itemName, int amount)
        {
            var item = GetItemDefinition(itemName).ToItem(this);
            for (var i = 0; i < amount; i++)
            {
                Items.Add(item);
            }

            if (_replicationManager != null && _replicationManager.IsServer())
            {
                _replicationManager.NotifyClientItemAdded(itemName, amount);
            }
            
            OnInventoryChanged?.Invoke();
        }

        public void RemoveItem(IBaseItem item)
        {
            if (Items.Remove(item))
            {
                if (_replicationManager != null && _replicationManager.IsServer())
                {
                    _replicationManager.NotifyClientItemRemoved(item.Name, 1);
                }
                OnInventoryChanged?.Invoke();
            }
        }

        public bool HasItem(IBaseItem item)
        {
            return Items.Contains(item);
        }

        public bool HasItemQuantity(IBaseItem item, int quantity)
        {
            var count = 0;
            foreach (var i in Items)
            {
                if (i.Name == item.Name)
                {
                    count++;
                }
            }
            return count >= quantity;
        }

        public bool HasItems(Dictionary<ItemDefinition, int> items)
        {
            foreach (var required in items)
            {
                var count = 0;
                foreach (var i in Items)
                {
                    if (i.Name == required.Key.Name)
                    {
                        count++;
                    }
                }
                if (count < required.Value) return false;
            }
            return true;
        }

        public void ConsumeItems(Dictionary<ItemDefinition, int> items)
        {
            if (!HasItems(items)) return;

            foreach (var required in items)
            {
                for (int j = 0; j < required.Value; j++)
                {
                    var itemToRemove = Items.FirstOrDefault(i => i.Name == required.Key.Name);
                    if (itemToRemove != null)
                    {
                        Items.Remove(itemToRemove);
                    }
                }

                if (_replicationManager != null && _replicationManager.IsServer())
                {
                    _replicationManager.NotifyClientItemRemoved(required.Key.Name, required.Value);
                }
            }
            OnInventoryChanged?.Invoke();
        }

        public ItemDefinition GetItemDefinition(string itemName)
        {
            return ItemLibrary.Instance.GetItemByName(itemName);
        }
    }
}
