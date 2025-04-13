using System;
using Systems.AbilitySystem.Attributes;
using Systems.AbilitySystem.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.Interface
{
    public class InterfaceController : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        private AbilitySystemComponent _asc;

        public void Initialise(AbilitySystemComponent owner)
        {
            _asc = owner;
            _asc.RegisterOnAttributeChanged("CharacteristicsAttributeSet","Health", OnHealthChanged);
            _asc.RegisterOnAttributeChanged("CharacteristicsAttributeSet","MaxHealth", OnHealthChanged);
            healthSlider.value = 1;
        }

        public void OnHealthChanged(AttributeBase attribute, float oldValue, float newValue)
        {
            float maxHealth = _asc.GetAttributeValue("CharacteristicsAttributeSet", "MaxHealth").Value.CurrentValue;
            float health = _asc.GetAttributeValue("CharacteristicsAttributeSet", "Health").Value.CurrentValue;
            healthSlider.value = health / maxHealth;
        }
        
        private void Update()
        {
            
        }
    }
}