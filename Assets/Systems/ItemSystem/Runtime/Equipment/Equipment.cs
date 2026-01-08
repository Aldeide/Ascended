using System.Collections.Generic;
using System.Linq;
using GameplayTags.Runtime;
using ItemSystem.Runtime.Database;
using ItemSystem.Runtime.Definition;
using ItemSystem.Runtime.Interface;
using ItemSystem.Runtime.Interface.Core;
using ItemSystem.Runtime.Manager;
using ItemSystem.Runtime.Modifiers;
using UnityEngine.Localization;

namespace ItemSystem.Runtime
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

        private readonly EquipmentDefinition _definition;
        private readonly IInventoryManager _manager;

        public Equipment(IInventoryManager manager, EquipmentDefinition definition)
        {
            Name = definition.Name;
            DisplayName = definition.DisplayName;
            Description = definition.Description;
            Mods = new Dictionary<Tag, Modifier>();
            _definition = definition;
            _manager = manager;

            InitialiseMods();
        }

        public Equipment(IInventoryManager manager, SerialisedEquipment serialisedEquipment)
        {
            _manager = manager;
            _definition = ItemLibrary.Instance.GetItemByName(serialisedEquipment.Name) as EquipmentDefinition;
            Level = serialisedEquipment.Level;
            Name = _definition.Name;
            DisplayName = _definition.DisplayName;
            Description = _definition.Description;
            Mods = new Dictionary<Tag, Modifier>();
            InitialiseMods();

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
            foreach (var ability in _definition.GrantedAbilities)
            {
                _manager.GetOwner().AbilityManager.GrantAbility(ability);
            }

            foreach (var effect in _definition.GrantedEffects)
            {
                _manager.GetOwner().EffectManager.AddEffect(effect.ToEffect(_manager.GetOwner(), _manager.GetOwner()));
            }
        }

        public void Unequip()
        {
            foreach (var ability in _definition.GrantedAbilities)
            {
                _manager.GetOwner().AbilityManager.RemoveAbility(ability);
            }
            foreach (var effect in _definition.GrantedEffects)
            {
                _manager.GetOwner().EffectManager.RemoveEffect(effect.name);
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
            if (slot.ModSlotTag == modSlot) return false;
            return slot.RequiredLevel <= Level;
        }

        public void Upgrade()
        {
            throw new System.NotImplementedException();
        }

        public bool CanUpgrade()
        {
            throw new System.NotImplementedException();
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