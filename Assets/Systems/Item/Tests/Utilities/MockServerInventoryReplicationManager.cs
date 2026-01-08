using Systems.Item.Runtime.Networking;


namespace Systems.Item.Tests.Utilities
{
    
    public class MockServerInventoryReplicationManager : IInventoryReplicationManager
    {
        public bool IsServer()
        {
            return true;
        }

        public bool IsClient()
        {
            return false;
        }

        public void NotifyClientAddItem(int itemId, int amount)
        {
            throw new System.NotImplementedException();
        }
    }
}