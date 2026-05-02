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
        [SerializeField] private Text _ammoCount;
        private AbilitySystemManager _asc;

        public void Initialise(AbilitySystemManager owner)
        {
            _asc = owner;
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Health", OnHealthChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxHealth", OnHealthChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("Energy", OnEnergyChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("MaxEnergy", OnEnergyChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("ClipSize", OnAmmoChanged);
            _asc.AttributeSetManager.RegisterOnAttributeChanged("CurrentClip", OnAmmoChanged);
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

        public void OnAmmoChanged(Attribute attribute, float oldValue, float newValue)
        {
            var current = _asc.AttributeSetManager.GetAttributeValue<WeaponAttributeSet>("CurrentClip").CurrentValue;
            var max = _asc.AttributeSetManager.GetAttributeValue<WeaponAttributeSet>("ClipSize").CurrentValue;
            _ammoCount.text = $"{current}/{max}";
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