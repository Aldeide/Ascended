using System;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystemExtension.Runtime.AttributeSets
{
    public class WeaponAttributeSet : AttributeSet
    {
        public Attribute ClipSize { get; private set; }
        public Attribute CurrentClip { get; private set; }
        public Attribute ReloadTime { get; private set; }
        public Attribute FireRate { get; private set; }
        public Attribute ReloadEnergyCost { get; private set; }
        public WeaponAttributeSet(IAbilitySystem owner) : base(owner)
        {
            Name = nameof(WeaponAttributeSet);
            ClipSize = new Attribute("ClipSize", this, 30);
            CurrentClip = new Attribute("CurrentClip", this, 30);
            ReloadTime = new Attribute("ReloadTime", this, 2);
            FireRate = new Attribute("FireRate", this, 10);
            ReloadEnergyCost = new Attribute("ReloadEnergyCost", this, 100);
            AddAttribute(ClipSize);
            AddAttribute(CurrentClip);
            AddAttribute(ReloadTime);
            AddAttribute(FireRate);
            AddAttribute(ReloadEnergyCost);
        }

        public override void Reset()
        {
            CurrentClip.SetBaseValue(ClipSize.BaseValue);
            CurrentClip.SetCurrentValue(ClipSize.BaseValue);
        }
    }
}
