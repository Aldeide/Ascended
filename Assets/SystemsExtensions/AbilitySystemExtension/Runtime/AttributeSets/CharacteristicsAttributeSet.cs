using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.AttributeSets
{
    public class CharacteristicsAttributeSet : AttributeSet
    {
        public Attribute Health { get; private set; }
        public Attribute MaxHealth { get; private set; }
        public Attribute Energy { get; private set; }
        public Attribute EnergyRegen { get; private set; }
        public Attribute MaxEnergy { get; private set; }
        public Attribute MovementSpeed { get; private set; }
        public Attribute Shield { get; private set; }
        public Attribute MaxShield { get; private set; }
        
        public CharacteristicsAttributeSet(IAbilitySystem owner) : base(owner)
        {
            Name = nameof(CharacteristicsAttributeSet);
            Health = new Attribute("Health", this,100);
            MaxHealth = new Attribute("MaxHealth", this,150);
            Energy = new Attribute("Energy", this,200);
            EnergyRegen = new Attribute("EnergyRegen", this,4);
            MaxEnergy = new Attribute("MaxEnergy", this,1000);
            MovementSpeed = new Attribute("MovementSpeed", this,4);
            Shield = new Attribute("Shield", this,0);
            MaxShield = new Attribute("MaxShield", this,0);

            AddAttribute(Health);
            AddAttribute(MaxHealth);
            AddAttribute(Energy);
            AddAttribute(EnergyRegen);
            AddAttribute(MaxEnergy);
            AddAttribute(MovementSpeed);
            AddAttribute(Shield);
            AddAttribute(MaxShield);
            
            Health.OnAttributeBaseValuePreChange += OnHealthChange;
            Health.OnAttributeCurrentValuePreChange += OnHealthChange;
            MaxHealth.OnAttributeCurrentValueChanged += OnMaxHealthChange;
            
            Energy.OnAttributeBaseValuePreChange += OnEnergyChange;
            Energy.OnAttributeCurrentValuePreChange += OnEnergyChange;
            MaxEnergy.OnAttributeCurrentValueChanged += OnMaxEnergyChange;
            
            Shield.OnAttributeBaseValuePreChange += OnShieldChange;
            Shield.OnAttributeCurrentValuePreChange += OnShieldChange;
            MaxShield.OnAttributeCurrentValueChanged += OnMaxShieldChange;
        }

        private float OnHealthChange(Attribute attribute, float nextValue)
        {
            if (nextValue <= 0)
            {
                _owner.AbilityManager.TryActivateAbility("DeathAbility");
            }
            var maxHealth = MaxHealth.CurrentValue;
            return Mathf.Min(nextValue, maxHealth);
        }
        
        private void OnMaxHealthChange(Attribute attribute, float previousValue, float nextValue)
        {
            if (Health.CurrentValue > nextValue) Health.SetCurrentValueNoEvent(nextValue);
            if (Health.BaseValue > nextValue) Health.SetBaseValueNoEvent(nextValue);
        }
        
        private float OnEnergyChange(Attribute attribute, float nextValue)
        {
            var maxEnergy = MaxEnergy.CurrentValue;
            return Mathf.Min(nextValue, maxEnergy);
        }
        
        private void OnMaxEnergyChange(Attribute attribute, float previousValue, float nextValue)
        {
            if (Energy.CurrentValue > nextValue) Energy.SetCurrentValueNoEvent(nextValue);
            if (Energy.BaseValue > nextValue) Energy.SetBaseValueNoEvent(nextValue);
        }
        
        private float OnShieldChange(Attribute attribute, float nextValue)
        {
            var maxShield = MaxShield.CurrentValue;
            return Mathf.Min(nextValue, maxShield);
        }
        
        private void OnMaxShieldChange(Attribute attribute, float previousValue, float nextValue)
        {
            if (Shield.CurrentValue > nextValue) Shield.SetCurrentValueNoEvent(nextValue);
            if (Shield.BaseValue > nextValue) Shield.SetBaseValueNoEvent(nextValue);
        }

        public override void Reset()
        {
            Health.SetBaseValue(MaxHealth.BaseValue);
            Health.SetCurrentValue(MaxHealth.BaseValue);
            Energy.SetBaseValue(MaxEnergy.BaseValue);
            Energy.SetCurrentValue(MaxEnergy.BaseValue);
            Shield.SetBaseValue(MaxShield.BaseValue);
            Shield.SetCurrentValue(MaxShield.BaseValue);
        }
    }
}
