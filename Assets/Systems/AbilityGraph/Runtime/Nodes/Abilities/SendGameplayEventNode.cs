using System;
using AbilityGraph.Runtime.Nodes.Base;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Scripts;
using GameplayTags.Runtime;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/Send Gameplay Event")]
    public class SendGameplayEventNode : LinearExecutableNode
    {
        [Input(name = "Target")]
        public GameObject Target;

        [Input(name = "Event Tag")]
        public Tag EventTag;

        [Input(name = "Magnitude")]
        public float Magnitude;

        protected override void Process()
        {
            var targetGo = Target != null ? Target : (Owner?.NetworkRole as Component)?.gameObject;
            if (targetGo == null) return;

            var asc = targetGo.GetComponent<AbilitySystemComponent>();
            if (asc != null && asc.AbilitySystem != null && asc.AbilitySystem.EventManager != null)
            {
                var gameEvent = new DynamicGameplayEvent(EventTag, Magnitude);
                asc.AbilitySystem.EventManager.TriggerEvent(gameEvent);
            }
        }
    }
}
