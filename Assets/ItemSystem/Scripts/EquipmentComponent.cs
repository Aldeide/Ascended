using AbilitySystem.Scripts;
using Assets.ItemSystem.Scripts;
using ItemSystem.Runtime.Manager;
using Unity.Netcode;
using UnityEngine;

namespace ItemSystem.Scripts
{
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class EquipmentComponent : NetworkBehaviour
    {
        public EquipmentSystemDefinition definition;
        
        private AbilitySystemComponent _abilitySystemComponent;
        //private InventoryComponent _inventoryComponent;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Initialise();
        }

        public void Initialise()
        {

            _abilitySystemComponent = GetComponent<AbilitySystemComponent>();
            //_inventoryComponent = GetComponent<InventoryComponent>();
            //EquipmentManager = new EquipmentManager(_abilitySystemComponent.AbilitySystem);
        }
    }
}