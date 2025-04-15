using AbilitySystem.Runtime.AttributeSet;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    public class AbilitySystemComponent : MonoBehaviour
    {
        private AttributeSetManager _attributeSetManager;
        private AbilitySystemManager _abilitySystem;
        private void Start()
        {
            _abilitySystem = new AbilitySystemManager();
            _attributeSetManager = new AttributeSetManager(_abilitySystem);
        }
    }
}