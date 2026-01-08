using System;
using System.Collections.Generic;
using System.Linq;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;

namespace Item.Runtime.Modifiers
{
    [Serializable]
    public struct ModSlot
    {
        [ValueDropdown("GetModSlotsDropdownValues", IsUniqueList = true, HideChildProperties = true)]
        public Tag ModSlotTag;
        public int RequiredLevel;
        public TagQuery TagQuery;
        
        private IEnumerable<ValueDropdownItem> GetModSlotsDropdownValues()
        {
            return TagsDropdown.GameplayTagChoices.Where(tag => tag.Text.StartsWith("Mod.Slot."));
        }
    }
}