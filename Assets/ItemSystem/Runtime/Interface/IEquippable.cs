using AbilitySystem.Runtime.Abilities;
using Assets.ItemSystem.Runtime.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ItemSystem.Runtime.Interface
{
    public interface IEquippable : IBaseItem
    {
        public List<EquipableSlot> Mods { get; }

        public Dictionary<ModType, int> MaxModByType { get; }

        public Ability EquippableAbility { get; }

        void EquipItem(); // -> Dans le manager

        void UnequipItem(); // -> Dans le manager

        public void SlotMod(IEquippableMod mod)
        {
            if(IsModSlotable(mod))
            {
                Mods.First(x => x.ModType == mod.Type).EquipableMods.Add(mod);
                //le calcule des ability des pieces d'equipement est fait au chargement de la run ?
                // est-ce qu'on peut changer d'equipement en cours de run ? (sloting, armure ?)
                // je suppose qu'on a le même système pour l'armure que pour l'arme ? on a qu'une armure qu'on slot pour upgradé
            }
        }

        public void UnSlotMod(IEquippableMod mod)
        {
            Mods.First(x => x.ModType == mod.Type).EquipableMods.Remove(mod);
            //pas besoin de remove l'ability in game car le sloting et unsloting sera fait dans le lobby ?
        }

        bool IsModSlotable(IEquippableMod mod)
        {
            EquipableSlot? equippableMod = Mods.FirstOrDefault(x => x.ModType == mod.Type);
            if (equippableMod is null)
            {
                equippableMod = new EquipableSlot()
                {
                    ModType = mod.Type,
                    EquipableMods = new List<IEquippableMod>(),
                    MaxSlots = MaxModByType[mod.Type]
                };
                Mods.Add(equippableMod.Value);
            }

            return equippableMod.Value.MaxSlots < equippableMod.Value.EquipableMods.Count;
        }

        public List<Ability> GetAbilities()
        {
            var results = new List<Ability>()
            {
                EquippableAbility
            };

            if(Mods is not null && Mods.Count() > 0)
            {
                foreach (var mod in Mods)
                {
                    if(mod.EquipableMods is not null && mod.EquipableMods.Count() > 0)
                        results.AddRange(mod.EquipableMods.Select(x => x.ModAbility));
                }
            }

            return results;
        } 
    }

    public struct EquipableSlot
    {
        public ModType ModType { get; set;}
        public int MaxSlots { get; set; }
        public List<IEquippableMod> EquipableMods { get; set; }
    }
}
