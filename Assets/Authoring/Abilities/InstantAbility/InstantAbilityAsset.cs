using System;
using Systems.AbilitySystem.Authoring;
using UnityEngine;

namespace Authoring.Abilities.InstantAbility
{
    public abstract class InstantAbilityAssetBase : AbilityAsset
    {
        
    }
    
    public abstract class InstantAbilityAssetT<T> : InstantAbilityAssetBase where T : class
    {
        public sealed override Type AbilityType()
        {
            return typeof(T);
        }
    }
    [CreateAssetMenu(fileName = "InstantAbility", menuName = "AbilitySystem/Abilities/InstantAbility")]
    public sealed class InstantAbilityAsset : InstantAbilityAssetT<InstantAbility>
    {
    }
}