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

        public override void ActivateAbility(params object[] args)
        {
            var target = (Vector3)args[1];
            var muzzle = (Vector3)args[0];
            var impact = ((FireAbilityDefinition)Definition).impactVisualEffect;
            PlayActivationCues();
            if (impact)
            {
                CueData data = new CueData();
                data.position = (Vector3)target;
                Owner.PlayCue(impact, data);
            }

            Ray ray = new Ray(muzzle, target - muzzle);
            RaycastHit hit;
            Debug.Log("Raycasting");
            if (Physics.Raycast(ray, out hit, 100f, ((FireAbilityDefinition)Definition).layerMask))
            {
                Debug.Log("Hit!");
                Debug.DrawLine(muzzle, hit.point, Color.red, 1.0f);
                var asc = hit.collider.GetComponent<AbilitySystemComponent>();
                if (!asc) return;
                asc.ExecuteEffect(((FireAbilityDefinition)Definition).damageEffect, Owner);
            }
        }

        public override void CancelAbility()
        {

        }

        public override void EndAbility()
        {

        }
    }
}