using System;
using System.Linq;
using AbilityGraph.Runtime.Nodes.Base;
using GameplayTags.Runtime;
using GraphProcessor;
using UnityEngine;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Abilities/Check Cooldown")]
    public class CheckCooldownNode : AbilityNode
    {
        [Input(name = "Cooldown Tag")]
        public Tag CooldownTag;

        [Output(name = "On Cooldown")]
        public bool OnCooldown;

        [Output(name = "Remaining Time")]
        public float RemainingTime;

        protected override void Process()
        {
            if (Owner == null || CooldownTag == null)
            {
                OnCooldown = false;
                RemainingTime = 0;
                return;
            }

            var activeEffectsWithTag = Owner.EffectManager.Effects.Where(e => 
                (e.Definition.AssetTags != null && e.Definition.AssetTags.Contains(CooldownTag)) ||
                (e.Definition.GrantedTags != null && e.Definition.GrantedTags.Contains(CooldownTag))
            ).ToList();

            if (activeEffectsWithTag.Count == 0)
            {
                OnCooldown = false;
                RemainingTime = 0;
                return;
            }

            OnCooldown = true;
            RemainingTime = activeEffectsWithTag.Max(e => e.RemainingDuration());
        }
    }
}
