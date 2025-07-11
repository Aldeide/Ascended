using System.Runtime.CompilerServices;
using System.Web;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Test.Utilities
{
    public class TestAttributeSet : AttributeSet
    {
        public Attribute Health;
        public Attribute MaxHealth;
        public Attribute Energy;
        public Attribute MaxEnergy;
        public Attribute MovementSpeed;
        public Attribute AbilityCost;
        public TestAttributeSet(IAbilitySystem owner) : base(owner)
        {
            Name = nameof(TestAttributeSet);
            Health = new Attribute("Health", this,100);
            MaxHealth = new Attribute("MaxHealth", this,150);
            Energy = new Attribute("Energy", this,200);
            MaxEnergy = new Attribute("MaxEnergy", this,300);
            MovementSpeed = new Attribute("MovementSpeed", this,4);
            AbilityCost = new Attribute("AbilityCost", this, 0);
            
            AddAttribute(Health);
            AddAttribute(MaxHealth);
            AddAttribute(Energy);
            AddAttribute(MaxEnergy);
            AddAttribute(MovementSpeed);
            AddAttribute(AbilityCost);
        }

        public override void Reset()
        {
            return;
        }
    }
}