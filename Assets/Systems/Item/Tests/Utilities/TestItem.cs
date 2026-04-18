using Item.Runtime.Definition;
using Item.Runtime.Interface;
using Item.Runtime.Modifiers;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using UnityEngine;
using UnityEngine.Localization;

namespace Systems.Item.Tests.Utilities
{
    public static class TestItems
    {
        public static TestItem BasicItem()
        {
            var itemDefinition = ScriptableObject.CreateInstance<EquipmentDefinition>();
            itemDefinition.Name = "BasicItem";
            return new TestItem(itemDefinition);
        }

        public static EquipmentDefinition BasicEquipment(string name, string slot)
        {
            var def = ScriptableObject.CreateInstance<EquipmentDefinition>();
            def.Name = name;
            def.EquipmentSlot = new GameplayTags.Runtime.Tag(slot);
            def.ModSlots = new ModSlot[0];
            def.GrantedAbilities = new AbilityDefinition[0];
            def.GrantedEffects = new EffectDefinition[0];
            def.EquipmentTags = new GameplayTags.Runtime.Tag[0];
            return def;
        }
    }
    public class TestItem : IBaseItem
    {
        public string Name { get; set; }
        public LocalizedString DisplayName { get; set; }
        public LocalizedString Description { get; set; }
        public UnityEngine.Sprite Icon { get; set; }
        
        public ItemDefinition Definition { get; private set; }
        
        public TestItem(ItemDefinition definition)
        {
            Name = definition.Name;
            DisplayName = definition.DisplayName;
            Description = definition.Description;
            Icon = definition.Icon;
            Definition = definition;
        }
    }
}