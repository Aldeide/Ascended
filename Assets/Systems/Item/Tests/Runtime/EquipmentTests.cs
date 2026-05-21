using Item.Runtime;
using Item.Runtime.Manager;
using Item.Runtime.Interface;
using Item.Runtime.Modifiers;
using NUnit.Framework;
using Moq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
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
            def.GrantedAbilities = new AbilityDefinition[0];
            def.GrantedEffects = new EffectDefinition[0];
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

        [Test]
        public void CanAddMod_SlotDoesNotExistOnEquipment_ReturnsFalse()
        {
            var inv = new Mock<IInventoryManager>();
            var declaredSlot = new Tag("Mod.Slot.Passive.1");
            var def = CreateEquipmentWithSlot(
                new Tag("Item.Equipment.Weapon"), declaredSlot, new Tag("Item.Modifier.Passive"));
            var equipment = new Equipment(inv.Object, def) { Level = 10 };

            var modDef = CreateModifierDef(new Tag("Item.Modifier.Passive"), new Tag("Item.Equipment.Weapon"));
            var mod = new Modifier(modDef, inv.Object);

            Assert.IsFalse(equipment.CanAddMod(new Tag("Mod.Slot.Passive.2"), mod));
        }

        [Test]
        public void AddMod_InvalidMod_LeavesSlotEmpty()
        {
            var inv = new Mock<IInventoryManager>();
            inv.Setup(m => m.GetOwner()).Returns(Source);
            var slot = new Tag("Mod.Slot.Passive.1");
            var def = CreateEquipmentWithSlot(
                new Tag("Item.Equipment.Weapon"), slot, new Tag("Item.Modifier.Passive"));
            var equipment = new Equipment(inv.Object, def) { Level = 10 };

            // Active mod placed in passive slot — CanAddMod should reject.
            var modDef = CreateModifierDef(new Tag("Item.Modifier.Active"), new Tag("Item.Equipment.Weapon"));
            var mod = new Modifier(modDef, inv.Object);

            equipment.AddMod(slot, mod);

            Assert.IsNull(equipment.Mods[slot], "Invalid mod must not populate the slot.");
        }

        [Test]
        public void AddMod_ValidMod_PopulatesSlotAndAttachesMod()
        {
            var inv = new Mock<IInventoryManager>();
            inv.Setup(m => m.GetOwner()).Returns(Source);
            var slot = new Tag("Mod.Slot.Passive.1");
            var def = CreateEquipmentWithSlot(
                new Tag("Item.Equipment.Weapon"), slot, new Tag("Item.Modifier.Passive"));
            var equipment = new Equipment(inv.Object, def) { Level = 10 };

            var modDef = CreateModifierDef(new Tag("Item.Modifier.Passive"), new Tag("Item.Equipment.Weapon"));
            var mod = new Modifier(modDef, inv.Object);

            equipment.AddMod(slot, mod);

            Assert.AreSame(mod, equipment.Mods[slot], "Valid mod should occupy the slot.");
        }

        [Test]
        public void RemoveMod_RemovesEntryFromMods()
        {
            var inv = new Mock<IInventoryManager>();
            inv.Setup(m => m.GetOwner()).Returns(Source);
            var slot = new Tag("Mod.Slot.Passive.1");
            var def = CreateEquipmentWithSlot(
                new Tag("Item.Equipment.Weapon"), slot, new Tag("Item.Modifier.Passive"));
            var equipment = new Equipment(inv.Object, def) { Level = 10 };

            var modDef = CreateModifierDef(new Tag("Item.Modifier.Passive"), new Tag("Item.Equipment.Weapon"));
            var mod = new Modifier(modDef, inv.Object);
            equipment.AddMod(slot, mod);
            Assert.AreSame(mod, equipment.Mods[slot]);

            equipment.RemoveMod(slot, mod);

            Assert.IsFalse(equipment.Mods.ContainsKey(slot),
                "RemoveMod should drop the slot entry from the Mods dictionary.");
        }

        [Test]
        public void Modifier_Properties_MirrorDefinitionTags()
        {
            var inv = new Mock<IInventoryManager>();
            var ownedTag = new Tag("Item.Modifier.Active");
            var modifiableTag = new Tag("Item.Equipment.EnergyCore");
            var modDef = CreateModifierDef(ownedTag, modifiableTag);

            var mod = new Modifier(modDef, inv.Object);

            CollectionAssert.AreEqual(new[] { ownedTag }, mod.OwnedTags);
            CollectionAssert.AreEqual(new[] { modifiableTag }, mod.ModifiableEquipmentTags);
        }

        [Test]
        public void AddMod_ReEquipAfterRemove_Succeeds()
        {
            var inv = new Mock<IInventoryManager>();
            inv.Setup(m => m.GetOwner()).Returns(Source);
            var slot = new Tag("Mod.Slot.Passive.1");
            var def = CreateEquipmentWithSlot(
                new Tag("Item.Equipment.Weapon"), slot, new Tag("Item.Modifier.Passive"));
            var equipment = new Equipment(inv.Object, def) { Level = 10 };

            var modDef = CreateModifierDef(new Tag("Item.Modifier.Passive"), new Tag("Item.Equipment.Weapon"));
            var mod = new Modifier(modDef, inv.Object);
            
            // Equip first time
            equipment.AddMod(slot, mod);
            Assert.AreSame(mod, equipment.Mods[slot]);

            // Remove it
            equipment.RemoveMod(slot, mod);
            Assert.IsFalse(equipment.Mods.ContainsKey(slot));

            // Equip again (re-equip)
            equipment.AddMod(slot, mod);
            Assert.AreSame(mod, equipment.Mods[slot], "Should be able to re-equip mod in the slot after removal.");
        }

        [Test]
        public void ToSerializedEquipment_WithEmptySlots_DoesNotThrowAndFiltersNulls()
        {
            var inv = new Mock<IInventoryManager>();
            inv.Setup(m => m.GetOwner()).Returns(Source);
            var slot = new Tag("Mod.Slot.Passive.1");
            var def = CreateEquipmentWithSlot(
                new Tag("Item.Equipment.Weapon"), slot, new Tag("Item.Modifier.Passive"));
            
            // Starts with slot set to null
            var equipment = new Equipment(inv.Object, def) { Level = 10, Name = "Sword" };

            SerialisedEquipment serialised = default;
            Assert.DoesNotThrow(() => serialised = equipment.ToSerializedEquipment(),
                "ToSerializedEquipment should not throw NullReferenceException when slots are empty.");

            Assert.IsNotNull(serialised.Modifiers);
            Assert.AreEqual(0, serialised.Modifiers.Count, "Empty slots should not be serialized.");
        }

        [Test]
        public void EquipmentManager_StartingEquipment_EquipsItems()
        {
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            var inv = new Mock<IInventoryManager>();
            inv.Setup(m => m.GetOwner()).Returns(Source);

            var def = CreateMockDefinition("MainHand", "Chest");
            
            var weaponDef = TestItems.BasicEquipment("StartingSword", "MainHand");
            var armorDef = TestItems.BasicEquipment("StartingShield", "Chest");

            var startingDef = UnityEngine.ScriptableObject.CreateInstance<global::Item.Runtime.Definition.StartingEquipmentDefinition>();
            startingDef.StartingEquipment = new List<EquipmentDefinition> { weaponDef, armorDef };

            var manager = new EquipmentManager(Source, inv.Object, def, startingEquipment: startingDef);

            var equipped = manager.GetEquipment();
            Assert.IsNotNull(equipped[new Tag("MainHand")]);
            Assert.AreEqual("StartingSword", equipped[new Tag("MainHand")].Name);
            
            Assert.IsNotNull(equipped[new Tag("Chest")]);
            Assert.AreEqual("StartingShield", equipped[new Tag("Chest")].Name);
        }

        [Test]
        public void EquipmentComponent_Initialise_PopulatesStartingEquipment()
        {
            var go = new UnityEngine.GameObject();
            var asc = go.AddComponent<AbilitySystem.Scripts.AbilitySystemComponent>();
            var abilitySystemField = typeof(AbilitySystem.Scripts.AbilitySystemComponent).GetProperty("AbilitySystem", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            abilitySystemField.SetValue(asc, Source);

            var inventoryComponent = go.AddComponent<global::Item.Scripts.InventoryComponent>();
            var mockInv = new Mock<IInventoryManager>();
            mockInv.Setup(m => m.GetOwner()).Returns(Source);
            inventoryComponent.InventoryManager = mockInv.Object;

            var equipmentComponent = go.AddComponent<global::Item.Scripts.EquipmentComponent>();
            
            var def = CreateMockDefinition("MainHand");
            var weaponDef = TestItems.BasicEquipment("StartingSword", "MainHand");
            var startingDef = UnityEngine.ScriptableObject.CreateInstance<global::Item.Runtime.Definition.StartingEquipmentDefinition>();
            startingDef.StartingEquipment = new List<EquipmentDefinition> { weaponDef };

            equipmentComponent.EquipmentManagerDefinition = def;
            equipmentComponent.StartingEquipment = startingDef;

            equipmentComponent.Initialise();

            Assert.IsNotNull(equipmentComponent.EquipmentManager);
            var equipped = equipmentComponent.EquipmentManager.GetEquipment();
            Assert.IsNotNull(equipped[new Tag("MainHand")]);
            Assert.AreEqual("StartingSword", equipped[new Tag("MainHand")].Name);

            UnityEngine.Object.DestroyImmediate(go);
        }
    }
}
