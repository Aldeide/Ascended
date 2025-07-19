using ItemSystem.Runtime.Interface;
using ItemSystem.Runtime.Interface.Core;
using UnityEngine;

namespace ItemSystem.Runtime.Definition
{
    [CreateAssetMenu(fileName = "CurrencyDefinition", menuName = "EquipmentSystem/CurrencyDefinition")]
    public class CurrencyDefinition : ItemDefinition
    {
        public int StackSize;
        public override IBaseItem ToItem(IInventoryManager inventoryManager)
        {
            return new Currency(this);
        }
    }
}