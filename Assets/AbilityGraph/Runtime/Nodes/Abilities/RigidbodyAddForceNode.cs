using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Abilities;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Character/RigidbodyAddForce")]
    public class RigidBodyAddForceNode : LinearExecutableNode
    {
        public Vector3 Force;
        public override string name => "Rigidbody: Add Force";
        private Rigidbody _rigidbody;
    
        public override void Initialise(Ability ability)
        {
            base.Initialise(ability);
            _rigidbody = ability.Owner.Component.gameObject.GetComponent<Rigidbody>();
        }
    
        protected override void Process()
        {
            _rigidbody.AddForce(Force, ForceMode.VelocityChange);
        }
    }
}