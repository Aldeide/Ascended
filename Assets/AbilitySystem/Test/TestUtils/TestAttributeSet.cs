using AbilitySystem.Runtime.Attribute;
using AbilitySystem.Runtime.AttributeSet;
using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Test.TestUtils
{
    public class TestAttributeSet : AttributeSet
    {
        public Attribute Health;
        public Attribute MaxHealth;
        public Attribute Energy;
        public Attribute MaxEnergy;
        public Attribute MovementSpeed;
        
        public TestAttributeSet(IAbilitySystem owner) : base(owner)
        {
            Name = nameof(TestAttributeSet);
            Health = new Attribute("Health", owner, this,100);
            MaxHealth = new Attribute("MaxHealth", owner, this,150);
            Energy = new Attribute("Energy", owner, this,200);
            MaxEnergy = new Attribute("MaxEnergy", owner, this,300);
            MovementSpeed = new Attribute("MovementSpeed", owner, this,4);
            
            AddAttribute(Health);
            AddAttribute(MaxHealth);
            AddAttribute(Energy);
            AddAttribute(MaxEnergy);
            AddAttribute(MovementSpeed);
        }
    }
}