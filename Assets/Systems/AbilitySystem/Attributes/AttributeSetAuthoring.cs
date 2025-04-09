using System.Collections.Generic;
using UnityEngine;

namespace Systems.Attributes
{
    [CreateAssetMenu(fileName = "AttributeSet", menuName = "AbilitySystem/AttributeSet", order = 1)]
    public class AttributeSetAuthoring : ScriptableObject
    {
        public string Name;
        public List<AttributeAuthoring> Attributes;
    }
}