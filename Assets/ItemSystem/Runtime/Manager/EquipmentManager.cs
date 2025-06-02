using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
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

        public void SlotMod(IEquippable equippable, IEquippableMod mod)
        {
            if (IsModSlotable(equippable, mod))
            {
                equippable.Mods.First(x => x.ModType == mod.Type).EquipableMods.Add(mod);
                //le calcule des ability des pieces d'equipement est fait au chargement de la run ?
                // est-ce qu'on peut changer d'equipement en cours de run ? (sloting, armure ?)
                // je suppose qu'on a le même système pour l'armure que pour l'arme ? on a
                // qu'une armure pour chaque emplacement qu'on slot pour upgradé
            }
        }

        public void UnSlotMod(IEquippable equippable, IEquippableMod mod)
        {
            equippable.Mods.First(x => x.ModType == mod.Type).EquipableMods.Remove(mod);
            //pas besoin de remove l'ability in game car le sloting et unsloting sera fait dans le lobby ?
        }

        public bool IsModSlotable(IEquippable equippable, IEquippableMod mod)
        {
            EquipableSlot? equippableMod = equippable.Mods.FirstOrDefault(x => x.ModType == mod.Type);
            if (equippableMod is null)
            {
                equippableMod = new EquipableSlot()
                {
                    ModType = mod.Type,
                    EquipableMods = new List<IEquippableMod>(),
                    MaxSlots = equippable.MaxModByType[mod.Type]
                };
                equippable.Mods.Add(equippableMod.Value);
            }

            return equippableMod.Value.MaxSlots < equippableMod.Value.EquipableMods.Count;
        }

        public List<Ability> GetAbilities(IEquippable equippable)
        {
            var results = new List<Ability>()
            {
                equippable.EquippableAbility
            };

            if (equippable.Mods is not null && equippable.Mods.Count() > 0)
            {
                foreach (var mod in equippable.Mods)
                {
                    if (mod.EquipableMods is not null && mod.EquipableMods.Count() > 0)
                        results.AddRange(mod.EquipableMods.Select(x => x.ModAbility));
                }
            }

            return results;
        }
    }
}