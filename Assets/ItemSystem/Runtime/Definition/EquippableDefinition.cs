using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using ItemSystem.Runtime.Constants;
using ItemSystem.Runtime.Interface;

namespace ItemSystem.Runtime.Definition
{
    public class EquippableDefinition : ItemDefinition, IEquippable
    {
        public EquippableSlotType SlotType { get; set; }

        public List<EquippableSlot> Mods { get; set; }

        public Dictionary<ModType, int> MaxModByType { get; set; }

        public AbilityDefinition[] EquippableAbilities { get; set; }

        public void SlotMod(EquippableModDefinition mod)
        {
            if (IsModSlotable(mod))
            {
                Mods.First(x => x.ModType == mod.Type).EquipableMods.Add(mod);
                //le calcule des ability des pieces d'equipement est fait au chargement de la run ?
                // est-ce qu'on peut changer d'equipement en cours de run ? (sloting, armure ?)
                // je suppose qu'on a le même système pour l'armure que pour l'arme ? on a
                // qu'une armure pour chaque emplacement qu'on slot pour upgradé
            }
        }

        public void UnSlotMod(EquippableModDefinition mod)
        {
            Mods.First(x => x.ModType == mod.Type).EquipableMods.Remove(mod);
            //pas besoin de remove l'ability in game car le sloting et unsloting sera fait dans le lobby ?
        }

        public bool IsModSlotable(EquippableModDefinition mod)
        {
            EquippableSlot? equippableSlot = Mods.FirstOrDefault(x => x.ModType == mod.Type);
            if (equippableSlot is null)
            {
                equippableSlot = new EquippableSlot()
                {
                    ModType = mod.Type,
                    EquipableMods = new List<IEquippableMod>(),
                    MaxSlots = MaxModByType[mod.Type]
                };
                Mods.Add(equippableSlot.Value);
            }

            return HasEnoughModSpace(equippableSlot.Value) && mod.IsSlotableInto(equippableSlot.Value);
        }

        public bool HasEnoughModSpace(EquippableSlot slot)
        {
            return slot.MaxSlots > slot.EquipableMods.Count;
        }
    }
}
