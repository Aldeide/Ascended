using System.Collections.Generic;
using GameplayTags.Runtime;
using Item.Runtime.Modifiers;

namespace Item.Runtime.Interface
{
    public interface IModifiable
    {
        public Dictionary<Tag, Modifier> Mods { get; }
        public void AddMod(Tag modSlot, Modifier mod);
        public void RemoveMod(Tag modSlot, Modifier mod);
        public bool CanAddMod(Tag modSlot, Modifier mod);
    }
}