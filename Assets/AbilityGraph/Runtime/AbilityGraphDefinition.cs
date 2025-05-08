using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilityGraph.Runtime
{
    [Serializable, CreateAssetMenu(fileName = "AbilityGraph", menuName = "AbilitySystem/Abilities/AbilityGraph")]
    public class AbilityGraphDefinition : AbilityDefinition
    {
        public AbilityGraph graph;
        
        public override Type AbilityType()
        {
            return typeof(TestAbilityGraph);
        }

        public override Ability ToAbility(IAbilitySystem owner)
        {
            return new TestAbilityGraph(this, owner);
        }
    }
}