using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using NewGraph;
using UnityEngine;

namespace AbilityGraph
{
    [CreateAssetMenu(fileName = "AbilityGraph", menuName = "AbilitySystem/Abilities/AbilityGraph")]
    public class AbilityGraphDefinition : AbilityDefinition
    {
        public ScriptableGraphModel abilityGraph;
        public override Type AbilityType()
        {
            return typeof(AbilityGraph);
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new AbilityGraph(this, owner);
        }
    }
}