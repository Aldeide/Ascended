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

        public WeaponAttributeSet(IAbilitySystem owner) : base(owner)
        {
            Name = nameof(WeaponAttributeSet);
            ClipSize = new Attribute("ClipSize", this, 100);
            CurrentClip = new Attribute("CurrentClip", this, 100);
            ReloadTime = new Attribute("ReloadTime", this, 100);

            AddAttribute(ClipSize);
            AddAttribute(CurrentClip);
            AddAttribute(ReloadTime);
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}