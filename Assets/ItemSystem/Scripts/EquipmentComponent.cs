using AbilitySystem.Scripts;
using Assets.ItemSystem.Scripts;
using ItemSystem.Runtime.Definition;
using ItemSystem.Runtime.Manager;
using Unity.Netcode;
using UnityEngine;

namespace ItemSystem.Scripts
{
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class EquipmentComponent : NetworkBehaviour
    {
        public EquipmentManager EquipmentManager;
        public EquipmentManagerDefinition EquipmentManagerDefinition;
        
        private AbilitySystemComponent _abilitySystemComponent;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _abilitySystemComponent = GetComponent<AbilitySystemComponent>();
            if (_abilitySystemComponent.IsInitialized)
            {
                Initialise();
            }
            else
            {
                _abilitySystemComponent.OnAbilitySystemInitialised += Initialise;
            }
            
        }

        public void Initialise()
        {
            EquipmentManager = new EquipmentManager(_abilitySystemComponent.AbilitySystem, EquipmentManagerDefinition);
        }
    }
}