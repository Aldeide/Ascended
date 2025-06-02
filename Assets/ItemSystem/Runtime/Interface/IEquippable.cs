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

        
    }

    public struct EquipableSlot
    {
        public ModType ModType { get; set;}
        public int MaxSlots { get; set; }
        public List<IEquippableMod> EquipableMods { get; set; }
    }
}
