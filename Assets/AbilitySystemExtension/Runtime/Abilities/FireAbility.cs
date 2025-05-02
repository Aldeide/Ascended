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
            var impact = ((FireAbilityDefinition)Definition).impactVisualEffect;
            PlayActivationCues();
            if (impact)
            {
                CueData data = new CueData();
                data.position = target;
                Owner.PlayCue(impact, data);
            }

            Ray ray = new Ray(muzzle, target - muzzle);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, ((FireAbilityDefinition)Definition).layerMask))
            {
                Debug.DrawLine(muzzle, hit.point, Color.red, 1.0f);
                var asc = hit.collider.GetComponent<AbilitySystemComponent>();
                if (!asc) return;
                asc.ExecuteEffect(((FireAbilityDefinition)Definition).damageEffect, Owner);
            }
        }

        protected override void CancelAbility()
        {

        }

        public override void EndAbility()
        {

        }
    }
}