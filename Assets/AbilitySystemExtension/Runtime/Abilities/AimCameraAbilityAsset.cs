using System;
using AbilitySystem.Runtime.Abilities;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    
    public class AimCameraAbilityAsset : AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(AimCameraAbilityDefinition);
        }
    }
}