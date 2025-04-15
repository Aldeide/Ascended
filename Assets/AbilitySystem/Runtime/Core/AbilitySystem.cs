using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using UnityEngine;

namespace AbilitySystem.Runtime.Core
{
    public class AbilitySystemManager : IAbilitySystem
    {
        GameplayTagManager IAbilitySystem.TagManager { get; set; }

        EffectManager IAbilitySystem.EffectManager { get; set; }
        
        AbilityManager IAbilitySystem.AbilityManager { get; set; }

        public AbilitySystemManager()
        {
            Debug.Log("Test");
        }
        
        public bool IsLocalClient()
        {
            return true;
        }


    }
}