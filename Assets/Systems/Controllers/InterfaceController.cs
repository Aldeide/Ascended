using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystemExtension.Runtime.AttributeSets;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.Controllers
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
            UpdateHealth();
        }

        public void OnHealthChanged(Attribute attribute, float oldValue, float newValue)
        {
            UpdateHealth();
        }
        
        private void UpdateHealth()
        {
            float maxHealth = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxHealth").CurrentValue;
            float health = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Health").CurrentValue;
            healthSlider.value = health / maxHealth;
        }

    }
}