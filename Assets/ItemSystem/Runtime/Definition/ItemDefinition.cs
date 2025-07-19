using System;
using ItemSystem.Runtime.Interface;
using UnityEngine;
using UnityEngine.Localization;

namespace ItemSystem.Runtime.Definition
{
    [Serializable]
    public class ItemDefinition : ScriptableObject, IBaseItem
    {
        [field:SerializeField]
        public LocalizedString DisplayName { get; set; }
        [field:SerializeField]
        public LocalizedString Description { get; set; }
    }
}
