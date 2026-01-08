using System;
using ItemSystem.Runtime.Interface.Core;
using Unity.Netcode;

namespace ItemSystem.Scripts
{
    public class InventoryComponent : NetworkBehaviour
    {
        public IInventoryManager InventoryManager { get; set; }


    }
}