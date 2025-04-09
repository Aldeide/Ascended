using UnityEngine;

namespace Systems.Attributes
{
    [CreateAssetMenu(fileName = "Attribute", menuName = "AbilitySystem/Attribute", order = 2)]
    public class AttributeAuthoring : ScriptableObject
    {
        public string Name;
        public float BaseValue;
        public float MinValue;
        public float MaxValue;
    }
}