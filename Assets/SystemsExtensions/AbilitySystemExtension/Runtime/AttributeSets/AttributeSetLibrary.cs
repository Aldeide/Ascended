using System;
using System.Collections.Generic;

namespace AbilitySystemExtension.Runtime.AttributeSets
{
    public static class AttributeSetLibrary
    {
        public static readonly Dictionary<string, Type> AttributeSetTypeDict = new Dictionary<string, Type>()
        {
            { "CharacteristicsAttributeSet", typeof(CharacteristicsAttributeSet) },
            { "WeaponAttributeSet", typeof(WeaponAttributeSet) },
            { "MetaAttributeSet", typeof(MetaAttributeSet) },
        };
        
        public static List<string> AttributeFullNames = new List<string>()
        {
            "CharacteristicsAttributeSet.Health",
            "CharacteristicsAttributeSet.MaxHealth",
            "CharacteristicsAttributeSet.Energy",
            "CharacteristicsAttributeSet.EnergyRegen",
            "CharacteristicsAttributeSet.MaxEnergy",
            "CharacteristicsAttributeSet.MovementSpeed",
            "CharacteristicsAttributeSet.Shield",
            "CharacteristicsAttributeSet.MaxShield",
            "WeaponAttributeSet.ClipSize",
            "WeaponAttributeSet.CurrentClip",
            "WeaponAttributeSet.ReloadTime",
            "WeaponAttributeSet.ReloadEnergyCost",
            "MetaAttributeSet.AbilityCooldown",
            "MetaAttributeSet.AbilityCharges",
            "MetaAttributeSet.AbilityCost",
            "MetaAttributeSet.EffectDuration",
            "MetaAttributeSet.DamageDone",
        };
    }
}
