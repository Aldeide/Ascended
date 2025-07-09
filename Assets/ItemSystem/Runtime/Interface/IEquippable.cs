using System.Collections.Generic;
using ItemSystem.Runtime.Constants;
using ItemSystem.Runtime.Definition;

namespace ItemSystem.Runtime.Interface
{
    public interface IEquippable : IBaseItem
    {
        void SlotMod(EquippableModDefinition mod);
        void UnSlotMod(EquippableModDefinition mod);
        bool IsModSlotable(EquippableModDefinition mod);
    }

    public struct EquippableSlot
    {
        public EquippableSlotType Type;
        public ModType ModType { get; set;}
        public int MaxSlots { get; set; }
        public List<IEquippableMod> EquipableMods { get; set; }
    }
}
