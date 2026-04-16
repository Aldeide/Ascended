using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Editor
{
    [Serializable]
    public class AttributeSetDefinition
    {
        public string SetName;
        public List<string> Attributes = new List<string>();
    }

    [CreateAssetMenu(fileName = "AttributeSetData", menuName = "AbilitySystem/Attribute Set Data")]
    public class AttributeSetData : ScriptableObject
    {
        public List<AttributeSetDefinition> AttributeSets = new List<AttributeSetDefinition>();
    }
}
