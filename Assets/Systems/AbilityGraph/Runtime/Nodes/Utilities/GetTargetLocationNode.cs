using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Abilities;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Utilities
{
    [Serializable, NodeMenuItem("Utilities/Get Target Location")]
    public class GetTargetLocationNode : AbilityNode
    {
        [Output(name = "Target Position")]
        public Vector3 TargetPosition;

        [Output(name = "Muzzle Position")]
        public Vector3 MuzzlePosition;

        protected override void Process()
        {
            if (Ability != null)
            {
                TargetPosition = Ability.Data.TargetPosition;
                MuzzlePosition = Ability.Data.MuzzlePosition;
                return;
            }
            
            TargetPosition = Vector3.zero;
            MuzzlePosition = Vector3.zero;
        }
    }
}
