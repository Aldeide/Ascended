using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystemExtension.Runtime.AttributeSets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Systems
{
    public class NameplateController : NetworkBehaviour
    {
        public Slider slider;
        
        private AbilitySystemManager _asc;

        public void Initialise(AbilitySystemManager owner)
        {
            _asc = owner;
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Health", OnHealthChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxHealth", OnHealthChanged);
            slider.value = 1;
            UpdateHealth();
        }

        private void OnHealthChanged(Attribute attribute, float oldValue, float newValue)
        {
            UpdateHealth();
        }

        private void UpdateHealth()
        {
            var maxHealth = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxHealth").CurrentValue;
            var health = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Health").CurrentValue;
            slider.value = health / maxHealth;
        }
    }
}