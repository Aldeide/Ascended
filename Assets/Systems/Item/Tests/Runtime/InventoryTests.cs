using AbilitySystem.Runtime.Core;
using Item.Runtime.Manager;
using NUnit.Framework;
using Moq;
using Systems.Item.Runtime.Networking;
using Systems.Item.Tests.Utilities;

namespace Systems.Item.Tests
{
    public class InventoryTests
    {
        [Test]
        public void InventoryTests_ServerAddItem_ItemIsAddedOnServer()
        {
            var owner = new Mock<IAbilitySystem>();
            var mockReplicationManager = new Mock<IInventoryReplicationManager>();
            mockReplicationManager.Setup(m => m.IsServer()).Returns(true);
            var inventory = new InventoryManager(owner.Object, mockReplicationManager.Object);
            
            inventory.AddItem(TestItems.BasicItem());
            
            // Checking the item has been added.
            Assert.AreEqual(1, inventory.Items.Count);
            Assert.AreEqual("BasicItem", inventory.Items[0].Name);
            mockReplicationManager.Verify(x => x.NotifyClientAddItem("BasicItem", 1), Times.Once);
        }
        
        [Test]
        public void InventoryTests_ServerAddItem_OwnerIsNotifiedOfItemAddition()
        {
            var owner = new Mock<IAbilitySystem>();
            var mockReplicationManager = new Mock<IInventoryReplicationManager>();
            mockReplicationManager.Setup(m => m.IsServer()).Returns(true);
            var inventory = new InventoryManager(owner.Object, mockReplicationManager.Object);
            
            inventory.AddItem(TestItems.BasicItem());
            
            mockReplicationManager.Verify(x => x.NotifyClientAddItem("BasicItem", 1), Times.Once);
        }

        [Test]
        public void InventoryTests_RemoveItem_ItemIsRemoved()
        {
            var owner = new Mock<IAbilitySystem>();
            var mockRepl = new Mock<IInventoryReplicationManager>();
            mockRepl.Setup(m => m.IsServer()).Returns(true);
            var inventory = new InventoryManager(owner.Object, mockRepl.Object);
            var item = TestItems.BasicItem();
            
            inventory.AddItem(item);
            Assert.AreEqual(1, inventory.Items.Count);
            
            inventory.RemoveItem(item);
            Assert.AreEqual(0, inventory.Items.Count);
        }

        [Test]
        public void InventoryTests_AddItem_FiresEvent()
        {
            var owner = new Mock<IAbilitySystem>();
            var mockRepl = new Mock<IInventoryReplicationManager>();
            mockRepl.Setup(m => m.IsServer()).Returns(true);
            var inventory = new InventoryManager(owner.Object, mockRepl.Object);
            bool eventFired = false;
            inventory.OnInventoryChanged += () => eventFired = true;

            inventory.AddItem(TestItems.BasicItem());

            Assert.IsTrue(eventFired, "OnInventoryChanged event should have been fired.");
        }

        [Test]
        public void InventoryTests_HasItemQuantity_ReturnsCorrectValue()
        {
            var owner = new Mock<IAbilitySystem>();
            var mockRepl = new Mock<IInventoryReplicationManager>();
            var inventory = new InventoryManager(owner.Object, mockRepl.Object);
            var item = TestItems.BasicItem();
            
            inventory.AddItem(item);
            inventory.AddItem(item);
            
            Assert.IsTrue(inventory.HasItemQuantity(item, 2));
            Assert.IsFalse(inventory.HasItemQuantity(item, 3));
        }

        [Test]
        public void InventoryTests_HasItems_ReturnsCorrectValue()
        {
            var owner = new Mock<IAbilitySystem>();
            var mockRepl = new Mock<IInventoryReplicationManager>();
            var inventory = new InventoryManager(owner.Object, mockRepl.Object);
            var item = TestItems.BasicItem();
            
            inventory.AddItem(item);
            
            var requirements = new System.Collections.Generic.Dictionary<global::Item.Runtime.Definition.ItemDefinition, int>
            {
                { item.Definition, 1 }
            };
            
            Assert.IsTrue(inventory.HasItems(requirements));
            
            requirements[item.Definition] = 2;
            Assert.IsFalse(inventory.HasItems(requirements));
        }

        [Test]
        public void InventoryTests_ConsumeItems_RemovesCorrectItems()
        {
            var owner = new Mock<IAbilitySystem>();
            var mockRepl = new Mock<IInventoryReplicationManager>();
            var inventory = new InventoryManager(owner.Object, mockRepl.Object);
            var item = TestItems.BasicItem();
            
            inventory.AddItem(item);
            inventory.AddItem(item);
            
            var toConsume = new System.Collections.Generic.Dictionary<global::Item.Runtime.Definition.ItemDefinition, int>
            {
                { item.Definition, 1 }
            };
            
            inventory.ConsumeItems(toConsume);
            
            Assert.AreEqual(1, inventory.Items.Count);
        }
    }
}