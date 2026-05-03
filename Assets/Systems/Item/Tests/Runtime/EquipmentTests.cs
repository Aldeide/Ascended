using Item.Runtime;
using Item.Runtime.Manager;
using Item.Runtime.Interface;
using Item.Runtime.Modifiers;
using NUnit.Framework;
using Moq;
using AbilitySystem.Runtime.Core;
using Systems.Item.Tests.Utilities;
using Item.Runtime.Interface.Core;
using Item.Runtime.Definition;
using GameplayTags.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace Systems.Item.Tests
{
    public class EquipmentTests
    {
        private EquipmentManagerDefinition CreateMockDefinition(params string[] slots)
        {
            var def = UnityEngine.ScriptableObject.CreateInstance<EquipmentManagerDefinition>();
            def.EquipmentSlots = slots.Select(s => new Tag(s)).ToArray();
            def.Equipment = new EquipmentDefinition[0];
            return def;
        }

        [Test]
        public void Equipment_EquipItem_UpdatesSlotAndFiresEvent()
        {
            var owner = new Mock<IAbilitySystem>();
            owner.Setup(m => m.IsServer()).Returns(true);
            var inv = new Mock<IInventoryManager>();
            var def = CreateMockDefinition("MainHand");
            
            var manager = new EquipmentManager(owner.Object, inv.Object, def);
            bool eventFired = false;
            manager.OnEquipmentChanged += () => eventFired = true;

            var item = TestItems.BasicEquipment("Sword", "MainHand");
            manager.Equip(new Tag("MainHand"), item);

            var eqp = manager.GetEquipment();
            Assert.AreEqual(item.Name, eqp[new Tag("MainHand")].Name);
            Assert.IsTrue(eventFired, "OnEquipmentChanged should have fired.");
        }

        [Test]
        public void Equipment_UnequipItem_ClearsSlot()
        {
            var owner = new Mock<IAbilitySystem>();
            owner.Setup(m => m.IsServer()).Returns(true);
            var inv = new Mock<IInventoryManager>();
            var def = CreateMockDefinition("MainHand");

            var manager = new EquipmentManager(owner.Object, inv.Object, def);
            var item = TestItems.BasicEquipment("Sword", "MainHand");
            
            manager.Equip(new Tag("MainHand"), item);
            Assert.IsNotNull(manager.GetEquipment()[new Tag("MainHand")]);

            manager.Unequip(new Tag("MainHand"));
            Assert.IsNull(manager.GetEquipment()[new Tag("MainHand")]);
        }

        [Test]
        public void Equipment_Upgrade_IncrementsLevelAndConsumesItems()
        {
            var inv = new Mock<IInventoryManager>();
            var def = UnityEngine.ScriptableObject.CreateInstance<EquipmentDefinition>();
            def.Name = "Sword";
            def.MaxLevel = 5;
            def.ModSlots = new global::Item.Runtime.Modifiers.ModSlot[0];
            
            var ingredient = UnityEngine.ScriptableObject.CreateInstance<EquipmentDefinition>();
            ingredient.Name = "Iron";

            var equipment = new Equipment(inv.Object, def);
            equipment.Level = 1;
            equipment.NextUpgradeCosts = new Dictionary<global::Item.Runtime.Definition.ItemDefinition, int> { { ingredient, 5 } };
            
            inv.Setup(m => m.HasItems(equipment.NextUpgradeCosts)).Returns(true);
            
            equipment.Upgrade();
            
            Assert.AreEqual(2, equipment.Level);
            inv.Verify(m => m.ConsumeItems(equipment.NextUpgradeCosts), Times.Once);
        }

        [Test]
        public void Equipment_CanUpgrade_ReturnsCorrectValue()
        {
            var inv = new Mock<IInventoryManager>();
            var def = UnityEngine.ScriptableObject.CreateInstance<EquipmentDefinition>();
            def.Name = "Sword";
            def.MaxLevel = 5;
            def.ModSlots = new global::Item.Runtime.Modifiers.ModSlot[0];

            var equipment = new Equipment(inv.Object, def);
            equipment.Level = 5; // Max level
            
            Assert.IsFalse(equipment.CanUpgrade(), "Should not be able to upgrade at max level.");
            
            equipment.Level = 1;
            equipment.NextUpgradeCosts = new Dictionary<global::Item.Runtime.Definition.ItemDefinition, int>();
            inv.Setup(m => m.HasItems(equipment.NextUpgradeCosts)).Returns(true);
            
            Assert.IsTrue(equipment.CanUpgrade());
        }

        private static EquipmentDefinition CreateEquipmentWithSlot(
            Tag equipmentTag, Tag modSlotTag, Tag requiredModTag, int requiredLevel = 0)
        {
            var def = UnityEngine.ScriptableObject.CreateInstance<EquipmentDefinition>();
            def.Name = "Eq";
            def.MaxLevel = 99;
            def.EquipmentTags = new[] { equipmentTag };
            def.ModSlots = new[]
            {
                new ModSlot
                {
                    ModSlotTag = modSlotTag,
                    RequiredLevel = requiredLevel,
                    TagQuery = new TagQuery(new TagCondition(TagMatchType.AnyOfExact, requiredModTag))
                }
            };
            return def;
        }

        private static ModifierDefinition CreateModifierDef(Tag ownedTag, Tag modifiableEquipmentTag)
        {
            var def = UnityEngine.ScriptableObject.CreateInstance<ModifierDefinition>();
            def.Name = "Mod";
            def.MaxLevel = 1;
            def.OwnedTags = new[] { ownedTag };
            def.ModifiableEquipmentTags = new[] { modifiableEquipmentTag };
            def.Recipe = new List<RecipeItem>();
            return def;
        }

        [Test]
        public void CanAddMod_ActiveModInPassiveSlot_ReturnsFalse()
        {
            var inv = new Mock<IInventoryManager>();
            var slot = new Tag("Mod.Slot.Passive.1");
            var def = CreateEquipmentWithSlot(
                new Tag("Item.Equipment.Weapon"), slot, new Tag("Item.Modifier.Passive"));
            var equipment = new Equipment(inv.Object, def) { Level = 10 };

            var modDef = CreateModifierDef(new Tag("Item.Modifier.Active"), new Tag("Item.Equipment.Weapon"));
            var mod = new Modifier(modDef, inv.Object);

            Assert.IsFalse(equipment.CanAddMod(slot, mod));
        }

        [Test]
        public void CanAddMod_PassiveModInActiveSlot_ReturnsFalse()
        {
            var inv = new Mock<IInventoryManager>();
            var slot = new Tag("Mod.Slot.Active.1");
            var def = CreateEquipmentWithSlot(
                new Tag("Item.Equipment.Weapon"), slot, new Tag("Item.Modifier.Active"));
            var equipment = new Equipment(inv.Object, def) { Level = 10 };

            var modDef = CreateModifierDef(new Tag("Item.Modifier.Passive"), new Tag("Item.Equipment.Weapon"));
            var mod = new Modifier(modDef, inv.Object);

            Assert.IsFalse(equipment.CanAddMod(slot, mod));
        }

        [Test]
        public void CanAddMod_ModWithIncompatibleEquipmentTag_ReturnsFalse()
        {
            var inv = new Mock<IInventoryManager>();
            var slot = new Tag("Mod.Slot.Passive.1");
            var def = CreateEquipmentWithSlot(
                new Tag("Item.Equipment.Weapon"), slot, new Tag("Item.Modifier.Passive"));
            var equipment = new Equipment(inv.Object, def) { Level = 10 };

            var modDef = CreateModifierDef(new Tag("Item.Modifier.Passive"), new Tag("Item.Equipment.EnergyCore"));
            var mod = new Modifier(modDef, inv.Object);

            Assert.IsFalse(equipment.CanAddMod(slot, mod));
        }

        [Test]
        public void CanAddMod_LevelBelowRequired_ReturnsFalse()
        {
            var inv = new Mock<IInventoryManager>();
            var slot = new Tag("Mod.Slot.Passive.2");
            var def = CreateEquipmentWithSlot(
                new Tag("Item.Equipment.Weapon"), slot, new Tag("Item.Modifier.Passive"), requiredLevel: 5);
            var equipment = new Equipment(inv.Object, def) { Level = 1 };

            var modDef = CreateModifierDef(new Tag("Item.Modifier.Passive"), new Tag("Item.Equipment.Weapon"));
            var mod = new Modifier(modDef, inv.Object);

            Assert.IsFalse(equipment.CanAddMod(slot, mod));
        }

        [Test]
        public void CanAddMod_AllConstraintsSatisfied_ReturnsTrue()
        {
            var inv = new Mock<IInventoryManager>();
            var slot = new Tag("Mod.Slot.Passive.1");
            var def = CreateEquipmentWithSlot(
                new Tag("Item.Equipment.Weapon"), slot, new Tag("Item.Modifier.Passive"));
            var equipment = new Equipment(inv.Object, def) { Level = 10 };

            var modDef = CreateModifierDef(new Tag("Item.Modifier.Passive"), new Tag("Item.Equipment.Weapon"));
            var mod = new Modifier(modDef, inv.Object);

            Assert.IsTrue(equipment.CanAddMod(slot, mod));
        }
    }
}
