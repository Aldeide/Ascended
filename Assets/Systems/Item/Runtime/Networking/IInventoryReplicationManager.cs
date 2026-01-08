namespace Systems.Item.Runtime.Networking
{
    public interface IInventoryReplicationManager
    {
        public bool IsServer();
        public bool IsClient();

        public void NotifyClientAddItem(int itemId, int amount);
    }
}