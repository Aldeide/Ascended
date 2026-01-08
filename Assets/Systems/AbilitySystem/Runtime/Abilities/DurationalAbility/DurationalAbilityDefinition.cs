using System;
using System.Collections.Generic;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.DurationalAbility
{
    [CreateAssetMenu(fileName = "DurationalAbility", menuName = "AbilitySystem/Abilities/DurationalAbility")]
    public class DurationalAbilityDefinition : AbilityDefinition
    {
        [SerializeReference]
        public List<InstantAbilityTask> ActivationTasks;
        [SerializeReference]
        public List<InstantAbilityTask> CancelTasks;
        [SerializeReference]
        public List<InstantAbilityTask> EndTasks;
        [SerializeReference]
        public List<AbilityTask> TickTasks;
        public override Type AbilityType()
        {
            return typeof(DurationalAbility);
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new DurationalAbility(this, owner);
        }
    }
}