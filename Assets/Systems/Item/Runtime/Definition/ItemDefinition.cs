using System;
using Item.Runtime.Interface;
using Item.Runtime.Interface.Core;
using UnityEngine;
using UnityEngine.Localization;

namespace Item.Runtime.Definition
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
