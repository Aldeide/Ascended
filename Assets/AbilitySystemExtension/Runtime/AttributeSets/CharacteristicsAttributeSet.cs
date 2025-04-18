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
        public Attribute MaxEnergy { get; private set; }
        public Attribute MovementSpeed { get; private set; }
        
        public CharacteristicsAttributeSet(IAbilitySystem owner) : base(owner)
        {
            Name = nameof(CharacteristicsAttributeSet);
            Health = new Attribute("Health", owner, this,100);
            MaxHealth = new Attribute("MaxHealth", owner, this,150);
            Energy = new Attribute("Energy", owner, this,200);
            MaxEnergy = new Attribute("MaxEnergy", owner, this,1000);
            MovementSpeed = new Attribute("MovementSpeed", owner, this,4);
            
            AddAttribute(Health);
            AddAttribute(MaxHealth);
            AddAttribute(Energy);
            AddAttribute(MaxEnergy);
            AddAttribute(MovementSpeed);

            Health.OnAttributeBaseValuePreChange += OnHealthChange;
            Health.OnAttributeCurrentValuePreChange += OnHealthChange;
            MaxHealth.OnAttributeCurrentValueChanged += OnMaxHealthChange;
        }

        private float OnHealthChange(Attribute attribute, float nextValue)
        {
            float maxHealth = MaxHealth.CurrentValue;
            return Mathf.Min(nextValue, maxHealth);
        }
        
        private void OnMaxHealthChange(Attribute attribute, float previousValue, float nextValue)
        {
            if (Health.CurrentValue > nextValue) Health.SetCurrentValueNoEvent(nextValue);
            if (Health.BaseValue > nextValue) Health.SetBaseValueNoEvent(nextValue);
        }
    }
}