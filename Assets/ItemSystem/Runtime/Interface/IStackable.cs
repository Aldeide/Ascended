namespace ItemSystem.Runtime.Interface
{
    public interface IStackable
    {
        public int StackSize { get; }
        public int CurrentStackSize { get; }
        public void AddToStack(int amount);
        public void RemoveFromStack(int amount);
    }
}