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
            Debug.Log("Init Nameplate");
            _asc = owner;
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Health", OnHealthChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxHealth", OnHealthChanged);
            slider.value = 1;
        }

        public void OnHealthChanged(Attribute attribute, float oldValue, float newValue)
        {
            Debug.Log("Health Changed");
            float maxHealth = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxHealth").CurrentValue;
            float health = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Health").CurrentValue;
            slider.value = health / maxHealth;
        }

        public void Update()
        {
            Debug.Log("Update nameplate");
            float maxHealth = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxHealth").CurrentValue;
            float health = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Health").CurrentValue;
            slider.value = health / maxHealth;
        }
    }
}