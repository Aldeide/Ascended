using System.Collections.Generic;
using System.Linq;
using GameplayTags.Runtime;
using Item.Runtime.Database;
using Item.Runtime.Definition;
using Item.Runtime.Interface;
using Item.Runtime.Interface.Core;
using Item.Runtime.Modifiers;
using Item.Runtime.Manager;
using UnityEngine.Localization;

namespace Item.Runtime
{
    public class Equipment : IBaseItem, IEquipable, IUpgradable, IModifiable
    {
        public string Name { get; set; }
        public LocalizedString DisplayName { get; set; }
        public LocalizedString Description { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; }
        public Dictionary<ItemDefinition, ScalableFloat.Runtime.ScalableFloat> UpgradeCosts { get; set; }
        public Dictionary<ItemDefinition, int> NextUpgradeCosts { get; set; }
        public Dictionary<Tag, Modifier> Mods { get; }
        public UnityEngine.Sprite Icon { get; set; }

        private readonly EquipmentDefinition _definition;
        private readonly IInventoryManager _manager;

        public Equipment(IInventoryManager manager, EquipmentDefinition definition)
        {
            Name = definition.Name;
            DisplayName = definition.DisplayName;
            Description = definition.Description;
            Mods = new Dictionary<Tag, Modifier>();
            _definition = definition;
            MaxLevel = definition.MaxLevel;
            _manager = manager;
            Icon = definition.Icon;

            InitialiseMods();
        }

        public Equipment(IInventoryManager manager, SerialisedEquipment serialisedEquipment)
        {
            _manager = manager;
            _definition = ItemLibrary.Instance.GetItemByName(serialisedEquipment.Name) as EquipmentDefinition;
            Level = serialisedEquipment.Level;
            MaxLevel = _definition.MaxLevel;
            Name = _definition.Name;
            DisplayName = _definition.DisplayName;
            Description = _definition.Description;
            Mods = new Dictionary<Tag, Modifier>();
            InitialiseMods();
            Icon = _definition.Icon;

            foreach (var mod in serialisedEquipment.Modifiers)
            {
                AddMod(new Tag(mod.Key), new Modifier(_manager, mod.Value));
            }
        }

        public SerialisedEquipment ToSerializedEquipment()
        {
            var modifiers = Mods.ToDictionary(mod => mod.Key.ToString(), mod => mod.Value.ToSerializedModifier());
            return new SerialisedEquipment()
            {
                Level = Level,
                Name = Name,
                Modifiers = modifiers,
            };
        }

        public void Equip()
        {
            var owner = _manager.GetOwner();
            if (owner == null)
            {
                UnityEngine.Debug.LogError($"Equipment {Name}: Cannot find owner via InventoryManager during Equip!");
                return;
            }

            foreach (var ability in _definition.GrantedAbilities)
            {
                owner.AbilityManager.GrantAbility(ability);
            }

            foreach (var effect in _definition.GrantedEffects)
            {
                owner.EffectManager.AddEffect(effect.ToEffect(owner, owner));
            }
        }

        public void Unequip()
        {
            var owner = _manager.GetOwner();
            if (owner == null)
            {
                UnityEngine.Debug.LogError($"Equipment {Name}: Cannot find owner via InventoryManager during Unequip!");
                return;
            }

            foreach (var ability in _definition.GrantedAbilities)
            {
                owner.AbilityManager.RemoveAbility(ability);
            }
            foreach (var effect in _definition.GrantedEffects)
            {
                owner.EffectManager.RemoveEffect(effect.name);
            }
        }

        public void AddMod(Tag modSlot, Modifier mod)
        {
            if (!CanAddMod(modSlot, mod)) return;
            if (!Mods.ContainsKey(modSlot)) return;
            Mods[modSlot] = mod;
            mod.Equip(this);
        }

        public void RemoveMod(Tag modSlot, Modifier mod)
        {
            Mods.Remove(modSlot);
            mod.Unequip(this);
        }

        public bool CanAddMod(Tag modSlot, Modifier mod)
        {
            var slot = _definition.ModSlots.FirstOrDefault(slot => slot.ModSlotTag == modSlot);
            if (slot.ModSlotTag != modSlot) return false;
            return slot.RequiredLevel <= Level;
        }

        public void Upgrade()
        {
            if (!CanUpgrade()) return;
            _manager.ConsumeItems(NextUpgradeCosts);
            Level++;
        }

        public bool CanUpgrade()
        {
            if (Level >= MaxLevel) return false;
            if (NextUpgradeCosts == null) return false;
            return _manager.HasItems(NextUpgradeCosts);
        }

        private void InitialiseMods()
        {
            foreach (var slot in _definition.ModSlots)
            {
                Mods.Add(slot.ModSlotTag, null);
            }
        }
    }
}