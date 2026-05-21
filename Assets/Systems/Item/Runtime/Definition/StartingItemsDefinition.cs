using System;
using System.Collections.Generic;
using UnityEngine;

namespace Item.Runtime.Definition
{
    [CreateAssetMenu(fileName = "StartingItemsDefinition", menuName = "EquipmentSystem/StartingItemsDefinition")]
    public class StartingItemsDefinition : ScriptableObject
    {
        [Serializable]
        public struct StartingItemEntry
        {
            public ItemDefinition Item;
            public int Quantity;
        }

        public List<StartingItemEntry> StartingItems;
    }
}
