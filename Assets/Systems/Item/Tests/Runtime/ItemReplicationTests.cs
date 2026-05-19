using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using Item.Runtime;
using Item.Runtime.Definition;
using Item.Runtime.Interface.Core;
using Item.Runtime.Manager;
using Item.Runtime.Modifiers;
using Moq;
using NUnit.Framework;
using Systems.Item.Runtime.Networking;
using Systems.Item.Tests.Utilities;
using UnityEngine;

namespace Systems.Item.Tests
{
    /// <summary>
    /// Tests for IItemReplicationManager fan-out: server-side mutations push
    /// outbound notifications and client-side Apply/Process methods replay
    /// state without re-firing the replication path.
    /// </summary>
    public class ItemReplicationTests : AbilitySystemTestBase
    {
        private static EquipmentDefinition CreateEquipmentDef(string name, string slotTag)
        {
            var def = ScriptableObject.CreateInstance<EquipmentDefinition>();
            def.Name = name;
            def.MaxLevel = 5;
            def.EquipmentSlot = new Tag(slotTag);
            def.EquipmentTags = new[] { new Tag("Item.Equipment.Weapon") };
            def.ModSlots = new ModSlot[0];
            def.GrantedAbilities = new AbilityDefinition[0];
            def.GrantedEffects = new EffectDefinition[0];
            return def;
        }

        private static EquipmentManagerDefinition CreateManagerDef(params string[] slots)
        {
            var def = ScriptableObject.CreateInstance<EquipmentManagerDefinition>();
            def.EquipmentSlots = slots.Select(s => new Tag(s)).ToArray();
            def.Equipment = new EquipmentDefinition[0];
            return def;
        }

        private static EquipmentManager BuildManager(
            IAbilitySystem owner,
            IInventoryManager inv,
            IItemReplicationManager rm,
            params string[] slots)
        {
            return new EquipmentManager(owner, inv, CreateManagerDef(slots), rm);
        }

        // --- Server-side: mutations push outbound notifications ---

        [Test]
        public void ServerEquip_InvokesNotifyClientEquipmentEquipped()
        {
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            var inv = new Mock<IInventoryManager>();
            inv.Setup(m => m.GetOwner()).Returns(Source);
            var rm = new Mock<IItemReplicationManager>();
            rm.Setup(m => m.IsServer()).Returns(true);
            var manager = BuildManager(Source, inv.Object, rm.Object, "MainHand");

            manager.Equip(new Tag("MainHand"), CreateEquipmentDef("Sword", "MainHand"));

            rm.Verify(m => m.NotifyClientEquipmentEquipped(
                It.Is<Tag>(t => t.Name == "MainHand"),
                It.Is<SerialisedEquipment>(s => s.Name == "Sword")), Times.Once);
        }

        [Test]
        public void ServerUnequip_InvokesNotifyClientEquipmentUnequipped()
        {
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            var inv = new Mock<IInventoryManager>();
            inv.Setup(m => m.GetOwner()).Returns(Source);
            var rm = new Mock<IItemReplicationManager>();
            rm.Setup(m => m.IsServer()).Returns(true);
            var manager = BuildManager(Source, inv.Object, rm.Object, "MainHand");

            manager.Equip(new Tag("MainHand"), CreateEquipmentDef("Sword", "MainHand"));
            manager.Unequip(new Tag("MainHand"));

            rm.Verify(m => m.NotifyClientEquipmentUnequipped(It.Is<Tag>(t => t.Name == "MainHand")), Times.Once);
        }

        [Test]
        public void ServerRemoveItem_InvokesNotifyClientItemRemoved()
        {
            var rm = new Mock<IItemReplicationManager>();
            rm.Setup(m => m.IsServer()).Returns(true);
            var inventory = new InventoryManager(Source, rm.Object);
            var item = TestItems.BasicItem();
            inventory.AddItem(item);

            inventory.RemoveItem(item);

            rm.Verify(m => m.NotifyClientItemRemoved("BasicItem", 1), Times.Once);
        }

        // --- Client-side: ApplyUnequipped does not re-fire replication ---

        [Test]
        public void ClientApplyUnequipped_DoesNotInvokeNotify()
        {
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            var inv = new Mock<IInventoryManager>();
            inv.Setup(m => m.GetOwner()).Returns(Source);
            var rm = new Mock<IItemReplicationManager>();
            rm.Setup(m => m.IsServer()).Returns(true);
            var manager = BuildManager(Source, inv.Object, rm.Object, "MainHand");

            // Server-side: equip first.
            manager.Equip(new Tag("MainHand"), CreateEquipmentDef("Sword", "MainHand"));
            rm.Invocations.Clear();

            // Client-side replay: must not re-fire NotifyClientEquipmentUnequipped.
            manager.ApplyUnequipped(new Tag("MainHand"));

            rm.Verify(m => m.NotifyClientEquipmentUnequipped(It.IsAny<Tag>()), Times.Never);
            Assert.IsNull(manager.GetEquipment()[new Tag("MainHand")]);
        }

        // --- Snapshot ---

        [Test]
        public void CaptureSnapshot_IncludesEquipmentAndInventory()
        {
            SourceMock.Setup(m => m.IsServer()).Returns(true);
            var rm = new Mock<IItemReplicationManager>();
            rm.Setup(m => m.IsServer()).Returns(true);
            var inv = new InventoryManager(Source, rm.Object);
            var manager = BuildManager(Source, inv, rm.Object, "MainHand");

            inv.AddItem(TestItems.BasicItem());
            manager.Equip(new Tag("MainHand"), CreateEquipmentDef("Sword", "MainHand"));

            var snapshot = manager.CaptureSnapshot();

            Assert.AreEqual(1, snapshot.Inventory["BasicItem"]);
            Assert.IsTrue(snapshot.Equipment.ContainsKey("MainHand"));
            Assert.AreEqual("Sword", snapshot.Equipment["MainHand"].Name);
        }

        // --- ItemReplicationManager: IsServer gate prevents Notify on client ---

        [Test]
        public void NotifyClientItemAdded_OnClient_Suppressed()
        {
            var rm = new ItemReplicationManager(() => false, () => true);
            int invokeCount = 0;
            rm.OnNotifyClientItemAdded = (_, _) => invokeCount++;

            rm.NotifyClientItemAdded("anything", 1);

            Assert.AreEqual(0, invokeCount, "Outbound notify must short-circuit on a client.");
        }

        [Test]
        public void NotifyClientItemAdded_OnServer_FiresDelegate()
        {
            var rm = new ItemReplicationManager(() => true, () => false);
            string capturedName = null;
            int capturedAmount = 0;
            rm.OnNotifyClientItemAdded = (n, a) => { capturedName = n; capturedAmount = a; };

            rm.NotifyClientItemAdded("Iron", 5);

            Assert.AreEqual("Iron", capturedName);
            Assert.AreEqual(5, capturedAmount);
        }
    }
}
