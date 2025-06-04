using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using Assets.ItemSystem.Runtime.Definition;
using Assets.ItemSystem.Runtime.Interface;

namespace ItemSystem.Runtime.Manager
{
    /// <summary>
    /// Responsible for managing Equipable Items. Handles the storage,
    /// granting, activation, deactivation, and lifecycle of abilities, while integrating
    /// with the associated <see cref="IAbilitySystem"/> owner.
    /// </summary>
    public class EquipmentManager
    {
        private IAbilitySystem _owner;
        
        private Dictionary<string, EquippableDefinition> _equipment = new();
        
        public EquipmentManager(IAbilitySystem owner)
        {
            _owner = owner;
        }
        public void Equip(string slotName, EquippableDefinition item)
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

        public List<AbilityDefinition> GetAbilities(EquippableDefinition equippable)
        {
            var results = new List<AbilityDefinition>();
            results.AddRange(equippable.EquippableAbilities);

            if (equippable.Mods is not null && equippable.Mods.Count() > 0)
            {
                foreach (var mod in equippable.Mods)
                {
                    if (mod.EquipableMods is not null && mod.EquipableMods.Count() > 0)
                        results.AddRange(mod.EquipableMods.Select(x => x.GetAbility()));
                }
            }

            return results;
        }
    }
}