using System;
using AbilitySystem.Runtime.Events;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.AbilityActivation
{
    [Serializable]
    public abstract class AbilityActivation
    {
        
    }
    [Serializable]
    public class OnGrantedActivation : AbilityActivation
    {
        
    }
    [Serializable]
    public class OnEventActivation : AbilityActivation
    {
        [SerializeReference]
        public GameplayEvent<EventArgs> activationEvent;
    }
}