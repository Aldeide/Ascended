using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using Assets.ItemSystem.Runtime.Interface;

namespace ItemSystem.Runtime.Manager
{
    public class EquipmentManager
    {
        private IAbilitySystem _owner;
        
        private Dictionary<string, IEquippable> _equipment = new();
        
        public EquipmentManager(IAbilitySystem owner)
        {
            _owner = owner;
        }

        public void Equip(string slotName, IEquippable item)
        {
            if (_equipment.ContainsKey(slotName))
            {
                _equipment[slotName] = item;
                return;
            }
            _equipment.TryAdd(slotName, item);
        }
        
        public void Unequip(string slotName)
        {
            _equipment.Remove(slotName);
        }
    }
}