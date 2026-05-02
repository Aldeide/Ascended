using AbilitySystem.Scripts;
using Item.Runtime.Interface.Core;
using Item.Runtime.Manager;
using Systems.Item.Runtime.Networking;
using Unity.Netcode;
using UnityEngine;

namespace Item.Scripts
{
    public class InventoryComponent : NetworkBehaviour
    {
        public IInventoryManager InventoryManager { get; set; }
        public ReplicationManager ReplicationManager { get; set; }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Initialise();
            if (IsServer)
            {
                // TODO: register events if any.
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsServer)
            {
                // TODO: unregister events if any.
            }
        }
        
        private void Initialise()
        {
            ReplicationManager = new ReplicationManager(this);
            InventoryManager = new InventoryManager(GetComponent<AbilitySystemComponent>().AbilitySystem, ReplicationManager);
        }

        [Rpc(SendTo.Owner)]
        public void NotifyOwnerAddItemRpc(string itemName, int amount)
        {
            if (!IsOwner) return;
            InventoryManager.AddItem(itemName, amount);
        }
    }
}