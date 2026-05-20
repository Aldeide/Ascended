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
        [Input("Force")]
        public Vector3 Force;
        public override string name => "Rigidbody: Add Force";
        private Rigidbody _rigidbody;
    
        public override void Initialise(GraphContext context)
        {
            base.Initialise(context);
            if (Ability != null && Ability.Owner != null && Ability.Owner.NetworkRole != null)
            {
                _rigidbody = ((Component)Ability.Owner.NetworkRole)?.gameObject.GetComponent<Rigidbody>();
            }
        }
    
        protected override void Process()
        {
            _rigidbody.AddForce(Force, ForceMode.VelocityChange);
        }
    }
}