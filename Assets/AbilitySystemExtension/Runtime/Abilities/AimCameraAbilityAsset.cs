using System;
using AbilitySystem.Runtime.Abilities;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    [CreateAssetMenu(fileName = "AimCameraAbility", menuName = "AbilitySystem/Abilities/AimCameraAbility")]
    public class AimCameraAbilityAsset : AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(AimCameraAbilityDefinition);
        }
    }
}