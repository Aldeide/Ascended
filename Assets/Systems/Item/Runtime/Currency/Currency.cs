using Item.Runtime.Definition;
using Item.Runtime.Interface;
using UnityEngine.Localization;

namespace Item.Runtime
{
    public class Currency : IBaseItem, IStackable
    {
        public string Name { get; set; }
        public LocalizedString DisplayName { get; set; }
        public LocalizedString Description { get; set; }
        
        public int StackSize { get; }
        public int CurrentStackSize { get; private set; }

        private CurrencyDefinition _definition;
        
        public Currency(CurrencyDefinition definition)
        {
            Name = definition.Name;
            DisplayName = definition.DisplayName;
            Description = definition.Description;
            StackSize = definition.StackSize;
            CurrentStackSize = 1;
            _definition = definition;
        }
        
        public void AddToStack(int amount)
        {
            if (CurrentStackSize + amount > StackSize)
            {
                CurrentStackSize = StackSize;
                return;
            }
            CurrentStackSize += amount;
        }

        public void RemoveFromStack(int amount)
        {
            if (CurrentStackSize - amount < 0)
            {
                CurrentStackSize = 0;
                return;
            }
            CurrentStackSize -= amount;
        }
    }
}