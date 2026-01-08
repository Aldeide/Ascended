using System.Collections.Generic;
using Item.Runtime.Database;
using Item.Runtime.Definition;
using Item.Runtime.Interface;
using Item.Runtime.Interface.Core;
using UnityEngine;
using UnityEngine.Localization;

namespace Item.Runtime.Modifiers
{
    public class Modifier : IBaseItem, IUpgradable
    {
        public string Name { get; set; }
        public LocalizedString DisplayName { get; set; }
        public LocalizedString Description { get; set; }
        public int Level { get; set; }
        public int MaxLevel { get; set; }
        
        public Dictionary<ItemDefinition, ScalableFloat.Runtime.ScalableFloat> UpgradeCosts { get; set; }
        public Dictionary<ItemDefinition, int> NextUpgradeCosts { get; set; }
        
        private readonly IInventoryManager _inventoryManager;
        private Equipment _attachedEquipment;
        private ModifierDefinition _definition;
        
        public Modifier(ModifierDefinition definition, IInventoryManager inventoryManager, int level = 1)
        {
            _inventoryManager = inventoryManager;
            _attachedEquipment = null;
            _definition = definition;
            DisplayName = definition.DisplayName;
            Description = definition.Description;
            Level = level;
            MaxLevel = definition.MaxLevel;
            SetUpgradeCosts(_definition);
        }

        public Modifier(IInventoryManager inventoryManager, SerialisedModifier serialisedModifier)
        {
            _inventoryManager = inventoryManager;
            _attachedEquipment = null;
            _definition = ItemLibrary.Instance.GetItemByName(serialisedModifier.Name) as ModifierDefinition;
            DisplayName = _definition.DisplayName;
            Description = _definition.Description;
            Level = serialisedModifier.Level;
            MaxLevel = _definition.MaxLevel;
            SetUpgradeCosts(_definition);
        }

        public SerialisedModifier ToSerializedModifier()
        {
            return new SerialisedModifier()
            {
                Level = Level,
                Name = Name,
            };
        }
        
        public void Upgrade()
        {
            if (!CanUpgrade()) return;
            Level++;
            _inventoryManager.ConsumeItems(NextUpgradeCosts);
            SetUpgradeCosts(ItemLibrary.Instance.GetItemByName(Name) as ModifierDefinition);
        }

        public bool CanUpgrade()
        {
            return Level != MaxLevel && _inventoryManager.HasItems(NextUpgradeCosts);
        }

        public void Equip(Equipment equipment)
        {
            _attachedEquipment = equipment;
            foreach (var ability in _definition.GrantedAbilities)
            {
                _inventoryManager.GetOwner().AbilityManager.GrantAbility(ability);
            }

            foreach (var effect in _definition.GrantedEffects)
            {
                var owner = _inventoryManager.GetOwner();
                _inventoryManager.GetOwner().EffectManager.AddEffect(effect.ToEffect(owner, owner));
            }
        }

        public void Unequip(Equipment equipment)
        {
            foreach (var ability in _definition.GrantedAbilities)
            {
                _inventoryManager.GetOwner().AbilityManager.RemoveAbility(ability);
            }
            foreach (var effect in _definition.GrantedEffects)
            {
                var owner = _inventoryManager.GetOwner();
                _inventoryManager.GetOwner().EffectManager.RemoveEffect(effect.name);
            }
        }

        private void SetUpgradeCosts(ModifierDefinition definition)
        {
            NextUpgradeCosts = new Dictionary<ItemDefinition, int>();
            foreach (var recipeItem in definition.Recipe)
            {
                var item = ItemLibrary.Instance.GetItemByName(recipeItem.Item.Name);
                var amount = Mathf.RoundToInt(recipeItem.Amount.Evaluate((float)Level / (float)MaxLevel));
                NextUpgradeCosts.Add(item, amount);
            }
        }
    }
}