using AbilitySystem.Runtime.Abilities;
using Assets.ItemSystem.Runtime.Constants;
using Assets.ItemSystem.Runtime.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Graphs;

namespace Assets.ItemSystem.Runtime.Interface
{
    public interface IEquippable : IBaseItem
    {
        void SlotMod(EquippableModDefinition mod);
        void UnSlotMod(EquippableModDefinition mod);
        bool IsModSlotable(EquippableModDefinition mod);
    }

    public struct EquipableSlot
    {
        public EquippableSlotType Type;
        public ModType ModType { get; set;}
        public int MaxSlots { get; set; }
        public List<IEquippableMod> EquipableMods { get; set; }
    }
}
