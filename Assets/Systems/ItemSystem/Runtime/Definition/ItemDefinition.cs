using System;
using ItemSystem.Runtime.Interface;
using ItemSystem.Runtime.Interface.Core;
using UnityEngine;
using UnityEngine.Localization;

namespace ItemSystem.Runtime.Definition
{
    [Serializable]
    public abstract class ItemDefinition : ScriptableObject
    {
        [Header("Display Information")] public string Name;
        public LocalizedString DisplayName;
        public LocalizedString Description;
        public Sprite Icon;

        public abstract IBaseItem ToItem(IInventoryManager inventoryManager);
    }
}
