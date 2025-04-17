using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using AbilitySystemExtension.Runtime.AttributeSets;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.Interface
{
    public class InterfaceController : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        private AbilitySystemManager _asc;

        public void Initialise(AbilitySystemManager owner)
        {
            _asc = owner;
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Health", OnHealthChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxHealth", OnHealthChanged);
            healthSlider.value = 1;
        }

        public void OnHealthChanged(Attribute attribute, float oldValue, float newValue)
        {
            float maxHealth = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxHealth").CurrentValue;
            float health = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Health").CurrentValue;
            healthSlider.value = health / maxHealth;
        }
        
        private void Update()
        {
            
        }

    }
}