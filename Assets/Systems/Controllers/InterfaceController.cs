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
        [SerializeField] private Slider _energySlider;
        private AbilitySystemManager _asc;

        public void Initialise(AbilitySystemManager owner)
        {
            _asc = owner;
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Health", OnHealthChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxHealth", OnHealthChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Energy", OnEnergyChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxEnergy", OnEnergyChanged);
            healthSlider.value = 1;
            UpdateHealth();
        }

        public void OnHealthChanged(Attribute attribute, float oldValue, float newValue)
        {
            UpdateHealth();
        }
        
        public void OnEnergyChanged(Attribute attribute, float oldValue, float newValue)
        {
            UpdateEnergy();
        }
        
        private void UpdateHealth()
        {
            var maxHealth = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxHealth").CurrentValue;
            var health = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Health").CurrentValue;
            healthSlider.value = health / maxHealth;
        }
        
        private void UpdateEnergy()
        {
            var maxEnergy = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("MaxEnergy").CurrentValue;
            var energy = _asc.AttributeSetManager.GetAttributeValue<CharacteristicsAttributeSet>("Energy").CurrentValue;
            _energySlider.value = energy / maxEnergy;
        }

    }
}