using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using GameplayTags.Runtime;
using ItemSystem.Runtime.Definition;

namespace ItemSystem.Runtime.Manager
{
    /// <summary>
    /// Responsible for managing Equipable Items. Handles the storage,
    /// granting, activation, deactivation, and lifecycle of abilities, while integrating
    /// with the associated <see cref="IAbilitySystem"/> owner.
    /// </summary>
    public class EquipmentManager
    {
        private readonly IAbilitySystem _owner;
        private EquipmentManagerDefinition _definition;
        
        private readonly Dictionary<Tag, Equipment> _equipment = new();
        
        public EquipmentManager(IAbilitySystem owner, EquipmentManagerDefinition definition)
        {
            _owner = owner;
            _definition = definition;
            foreach (var slot in _definition.EquipmentSlots)
            {
                _equipment.TryAdd(slot, null);
            }

            foreach (var equipmentDefinition in _definition.Equipment)
            {
                var slot = equipmentDefinition.EquipmentSlot;
                if (_equipment.ContainsKey(slot))
                {
                    var equipment = new Equipment(equipmentDefinition, this);
                    _equipment[slot] = equipment;
                    _equipment[slot].Equip();
                }
            }
        }
        public void Equip(Tag slotName, EquipmentDefinition item)
        {
            if (!_equipment.ContainsKey(slotName)) return;
            _equipment[slotName] = new Equipment(item, this);;
        }

        public void Unequip(Tag slotName)
        {
            _equipment[slotName].Unequip();
            _equipment[slotName] = null;
        }
        
        public IAbilitySystem GetOwner()
        {
            return _owner;
        }
    }
}