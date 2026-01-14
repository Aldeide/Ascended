using Item.Scripts;

namespace Systems.Item.Runtime.Networking
{
    public class ReplicationManager : IInventoryReplicationManager
    {
        private readonly InventoryComponent _inventoryComponent;
        public ReplicationManager(InventoryComponent inventoryComponent)
        {
            _inventoryComponent = inventoryComponent;
        }
        
        public bool IsServer()
        {
            return _inventoryComponent.IsServer;
        }

        public bool IsClient()
        {
            return _inventoryComponent.IsClient;
        }

        public void NotifyClientAddItem(string itemName, int amount)
        {
            _inventoryComponent.NotifyOwnerAddItemRpc(itemName, amount);
        }
    }
}