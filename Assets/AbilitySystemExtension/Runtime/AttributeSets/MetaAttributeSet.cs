using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;

namespace AbilitySystemExtension.Runtime.AttributeSets
{
    public class MetaAttributeSet : AttributeSet
    {
        public Attribute AbilityCooldown { get; private set; }
        public Attribute AbilityCost { get; private set; }
        public Attribute EffectDuration { get; private set; }
        
        public MetaAttributeSet(IAbilitySystem owner) : base(owner)
        {
            Name = nameof(MetaAttributeSet);
            AbilityCooldown = new Attribute("AbilityCooldown", owner, this,0);
            AbilityCost = new Attribute("AbilityCost", owner, this,0);
            EffectDuration = new Attribute("EffectDuration", owner, this,0);
            
            AddAttribute(AbilityCooldown);
            AddAttribute(AbilityCost);
            AddAttribute(EffectDuration);
        }
    }
}