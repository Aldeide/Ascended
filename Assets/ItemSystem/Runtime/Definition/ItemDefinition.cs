using System;
using ItemSystem.Runtime.Interface;
using UnityEngine;

namespace ItemSystem.Runtime.Definition
{
    [Serializable]
    public class ItemDefinition : ScriptableObject, IBaseItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
