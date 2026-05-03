using AbilitySystem.Runtime.Core;
using AbilitySystem.Test.Utilities;
using Item.Runtime.Manager;
using NUnit.Framework;
using Moq;
using Systems.Item.Runtime.Networking;
using Systems.Item.Tests.Utilities;
using System.Collections.Generic;

namespace Systems.Item.Tests
{
    /// <summary>
    /// Tests for the Inventory system, including item addition/removal, quantity checks, 
    /// and server-client replication triggers.
    /// </summary>
    public class InventoryTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Validates that adding an item on the server correctly updates the inventory list 
        /// and triggers a client notification.
        /// </summary>
        [Test]
        public void InventoryTests_AddItemOnServer_ItemIsAddedAndClientNotified()
        {
            var mockReplicationManager = new Mock<IInventoryReplicationManager>();
            mockReplicationManager.Setup(m => m.IsServer()).Returns(true);
            var inventory = new InventoryManager(Source, mockReplicationManager.Object);
            
            inventory.AddItem(TestItems.BasicItem());
            
            Assert.AreEqual(1, inventory.Items.Count);
            Assert.AreEqual("BasicItem", inventory.Items[0].Name);
            mockReplicationManager.Verify(x => x.NotifyClientAddItem("BasicItem", 1), Times.Once);
        }

        /// <summary>
        /// Validates that removing an item correctly updates the inventory state.
        /// </summary>
        [Test]
        public void InventoryTests_RemoveItem_ItemIsRemovedFromInventory()
        {
            var mockRepl = new Mock<IInventoryReplicationManager>();
            mockRepl.Setup(m => m.IsServer()).Returns(true);
            var inventory = new InventoryManager(Source, mockRepl.Object);
            var item = TestItems.BasicItem();
            
            inventory.AddItem(item);
            Assert.AreEqual(1, inventory.Items.Count);
            
            inventory.RemoveItem(item);
            Assert.AreEqual(0, inventory.Items.Count);
        }

        /// <summary>
        /// Validates that the OnInventoryChanged event fires when an item is added.
        /// </summary>
        [Test]
        public void InventoryTests_AddItem_FiresOnInventoryChangedEvent()
        {
            var mockRepl = new Mock<IInventoryReplicationManager>();
            mockRepl.Setup(m => m.IsServer()).Returns(true);
            var inventory = new InventoryManager(Source, mockRepl.Object);
            bool eventFired = false;
            inventory.OnInventoryChanged += () => eventFired = true;

            inventory.AddItem(TestItems.BasicItem());

            Assert.IsTrue(eventFired, "OnInventoryChanged event should have been fired.");
        }

        /// <summary>
        /// Validates that HasItemQuantity correctly reports availability based on count.
        /// </summary>
        [Test]
        public void InventoryTests_HasItemQuantity_ReturnsCorrectStatus()
        {
            var mockRepl = new Mock<IInventoryReplicationManager>();
            var inventory = new InventoryManager(Source, mockRepl.Object);
            var item = TestItems.BasicItem();
            
            inventory.AddItem(item);
            inventory.AddItem(item);
            
            Assert.IsTrue(inventory.HasItemQuantity(item, 2));
            Assert.IsFalse(inventory.HasItemQuantity(item, 3));
        }

        /// <summary>
        /// Validates that HasItems correctly reports eligibility for a collection of requirements.
        /// </summary>
        [Test]
        public void InventoryTests_HasItems_ReturnsCorrectStatusForRequirements()
        {
            var mockRepl = new Mock<IInventoryReplicationManager>();
            var inventory = new InventoryManager(Source, mockRepl.Object);
            var item = TestItems.BasicItem();
            
            inventory.AddItem(item);
            
            var requirements = new Dictionary<global::Item.Runtime.Definition.ItemDefinition, int>
            {
                { item.Definition, 1 }
            };
            
            Assert.IsTrue(inventory.HasItems(requirements));
            
            requirements[item.Definition] = 2;
            Assert.IsFalse(inventory.HasItems(requirements));
        }

        /// <summary>
        /// Validates that ConsumeItems correctly removes the specified quantities of items.
        /// </summary>
        [Test]
        public void InventoryTests_ConsumeItems_RemovesCorrectQuantities()
        {
            var mockRepl = new Mock<IInventoryReplicationManager>();
            var inventory = new InventoryManager(Source, mockRepl.Object);
            var item = TestItems.BasicItem();
            
            inventory.AddItem(item);
            inventory.AddItem(item);
            
            var toConsume = new Dictionary<global::Item.Runtime.Definition.ItemDefinition, int>
            {
                { item.Definition, 1 }
            };
            
            inventory.ConsumeItems(toConsume);
            
            Assert.AreEqual(1, inventory.Items.Count);
        }
    }
}