using System.Collections.Generic;
using AbilitySystem.Runtime.Abilities;
using ItemSystem.Runtime.Constants;
using ItemSystem.Runtime.Interface;

namespace ItemSystem.Runtime.Definition
{
    public class EquippableModDefinition : ItemDefinition, IEquippableMod
    {
        public ModType Type { get; }

        private bool IsModEnabled { get; set; }

        private List<EquippableSlotType> RestrictedSlot { get; set; }

        public AbilityDefinition ModAbility { get; }

        public AbilityDefinition GetAbility()
        {
            return IsModEnabled ? ModAbility : null;
        }

        public void DisableMod()
        {
            IsModEnabled = false;
        }

        public void EnableMod()
        {
            IsModEnabled = true;
        }

        public bool IsSlotableInto(EquippableSlot slot)
        {
            return slot.ModType == Type
                && (RestrictedSlot is null || RestrictedSlot.Count == 0 || RestrictedSlot.Contains(slot.Type));
        }
    }
}
