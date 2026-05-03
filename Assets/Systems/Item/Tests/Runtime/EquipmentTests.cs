using Item.Runtime;
using Item.Runtime.Manager;
using NUnit.Framework;
using Moq;
using AbilitySystem.Runtime.Core;
using Systems.Item.Tests.Utilities;
using Item.Runtime.Interface.Core;
using Item.Runtime.Definition;
using GameplayTags.Runtime;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Test.Utilities;
using UnityEngine;

namespace Systems.Item.Tests
{
    /// <summary>
    /// Tests for the Equipment system, including item equipping, unequipping, and upgrade logic.
    /// </summary>
    public class EquipmentTests : AbilitySystemTestBase
    {
        private EquipmentManagerDefinition CreateMockDefinition(params string[] slots)
        {
            var def = ScriptableObject.CreateInstance<EquipmentManagerDefinition>();
            def.EquipmentSlots = slots.Select(s => new Tag(s)).ToArray();
            def.Equipment = new EquipmentDefinition[0];
            return def;
        }

        /// <summary>
        /// Validates that equipping an item correctly updates the target slot and triggers the change event.
        /// </summary>
        [Test]
        public void EquipmentTests_EquipItem_UpdatesSlotAndFiresEvent()
        {
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            var inv = new Mock<IInventoryManager>();
            inv.Setup(m => m.GetOwner()).Returns(Source); 
            
            var def = CreateMockDefinition("MainHand");
            
            var manager = new EquipmentManager(Source, inv.Object, def);
            bool eventFired = false;
            manager.OnEquipmentChanged += () => eventFired = true;

            var item = TestItems.BasicEquipment("Sword", "MainHand");
            manager.Equip(new Tag("MainHand"), item);

            var eqp = manager.GetEquipment();
            Assert.AreEqual(item.Name, eqp[new Tag("MainHand")].Name);
            Assert.IsTrue(eventFired, "OnEquipmentChanged should have fired.");
        }

        /// <summary>
        /// Validates that unequipping an item correctly clears the target slot.
        /// </summary>
        [Test]
        public void EquipmentTests_UnequipItem_ClearsSlot()
        {
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            var inv = new Mock<IInventoryManager>();
            inv.Setup(m => m.GetOwner()).Returns(Source); 
            
            var def = CreateMockDefinition("MainHand");

            var manager = new EquipmentManager(Source, inv.Object, def);
            var item = TestItems.BasicEquipment("Sword", "MainHand");
            
            manager.Equip(new Tag("MainHand"), item);
            Assert.IsNotNull(manager.GetEquipment()[new Tag("MainHand")]);

            manager.Unequip(new Tag("MainHand"));
            Assert.IsNull(manager.GetEquipment()[new Tag("MainHand")]);
        }

        /// <summary>
        /// Validates that upgrading equipment correctly increments its level and consumes required ingredients.
        /// </summary>
        [Test]
        public void EquipmentTests_Upgrade_IncrementsLevelAndConsumesItems()
        {
            var inv = new Mock<IInventoryManager>();
            var def = ScriptableObject.CreateInstance<EquipmentDefinition>();
            def.Name = "Sword";
            def.MaxLevel = 5;
            def.ModSlots = new global::Item.Runtime.Modifiers.ModSlot[0];
            
            var ingredient = ScriptableObject.CreateInstance<EquipmentDefinition>();
            ingredient.Name = "Iron";

            var equipment = new Equipment(inv.Object, def);
            equipment.Level = 1;
            equipment.NextUpgradeCosts = new Dictionary<global::Item.Runtime.Definition.ItemDefinition, int> { { ingredient, 5 } };
            
            inv.Setup(m => m.HasItems(equipment.NextUpgradeCosts)).Returns(true);
            
            equipment.Upgrade();
            
            Assert.AreEqual(2, equipment.Level);
            inv.Verify(m => m.ConsumeItems(equipment.NextUpgradeCosts), Times.Once);
        }

        /// <summary>
        /// Validates the CanUpgrade method correctly reports upgrade eligibility based on level and ingredients.
        /// </summary>
        [Test]
        public void EquipmentTests_CanUpgrade_ReturnsCorrectStatus()
        {
            var inv = new Mock<IInventoryManager>();
            var def = ScriptableObject.CreateInstance<EquipmentDefinition>();
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
    }
}
