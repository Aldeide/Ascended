using System;
using Systems.AbilitySystem.Authoring;
using Systems.AbilitySystem.Authoring.AbilityTasks;
using UnityEngine;

namespace Systems.AbilitySystem.Abilities.InstantAbility
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
        public AbilityTaskAsset[] OnActivateTasks;
        public AbilityTaskAsset[] OnEndTasks;
    }
}