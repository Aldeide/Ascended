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
            // TODO: change after implementing item database.
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
            
            // TODO: change after implementing item database.
            mockReplicationManager.Verify(x => x.NotifyClientAddItem("BasicItem", 1), Times.Once);
        }
    }
}