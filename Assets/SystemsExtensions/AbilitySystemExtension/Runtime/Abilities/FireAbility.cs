using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class FireAbility : Ability
    {
        public FireAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        protected override void ActivateAbility(AbilityData abilityData)
        {
            var target = abilityData.TargetPosition;
            var muzzle = abilityData.MuzzlePosition;
            var trail = ((FireAbilityDefinition)Definition).trailVisualEffect;
            var impact = ((FireAbilityDefinition)Definition).impactVisualEffect;
            PlayActivationCues();
            
            var ray = new Ray(muzzle, target - muzzle);
            if (!Physics.Raycast(ray, out var hit, 100f, ((FireAbilityDefinition)Definition).layerMask)) return;
            Debug.DrawLine(muzzle, hit.point, Color.red, 1.0f);
            
            if (impact)
            {
                var data = new CueData
                {
                    VectorData = new[] {hit.point, muzzle, hit.normal}
                };
                Debug.Log("FireAbility: Sending impact cue");
                Owner.PlayCue(impact, data, true);
            }

            if (trail)
            {
                var data = new CueData
                {
                    VectorData = new[] {hit.point, muzzle, hit.normal}
                };
                Debug.Log("FireAbility: Sending trail cue");
                Owner.PlayCue(trail, data, true);
            }
            
            var asc = hit.collider.GetComponent<AbilitySystemComponent>();
            if (!asc) return;
            asc.ExecuteEffect(((FireAbilityDefinition)Definition).damageEffect, Owner);
        }

        protected override void CancelAbility()
        {

        }

        public override void EndAbility()
        {

        }
    }
}
