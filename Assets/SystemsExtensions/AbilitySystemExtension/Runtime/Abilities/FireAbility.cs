using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using AbilitySystemExtension.Runtime.AttributeSets;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class FireAbility : Ability
    {
        public FireAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        public override AbilityActivationResult CanActivate()
        {
            if (IsActive) return AbilityActivationResult.BlockedByAbility;
            
            var weaponSet = Owner.AttributeSetManager.GetAttributeSet<WeaponAttributeSet>();
            if (weaponSet != null && weaponSet.CurrentClip.CurrentValue <= 0)
            {
                return AbilityActivationResult.MissingRequiredTag;
            }
            return base.CanActivate();
        }

        protected override void ActivateAbility(AbilityData abilityData)
        {
            var weaponSet = Owner.AttributeSetManager.GetAttributeSet<WeaponAttributeSet>();
            if (weaponSet == null)
            {
                Debug.LogError("FireAbility: WeaponAttributeSet not found on owner!");
                EndAbility();
                return;
            }

            if (weaponSet.CurrentClip.CurrentValue <= 0)
            {
                if (Owner.IsLocalClient())
                {
                    Owner.AbilityManager.TryActivateAbility("ReloadWeaponAbility");
                }
                EndAbility();
                return;
            }

            // Consume ammo
            weaponSet.CurrentClip.SetCurrentValue(weaponSet.CurrentClip.CurrentValue - 1);

            PlayActivationCues();
            
            var target = abilityData.TargetPosition;
            var muzzle = abilityData.MuzzlePosition;
            var ray = new Ray(muzzle, target - muzzle);

            // Raycast and damage logic
            if (Physics.Raycast(ray, out var hit, 100f, ((FireAbilityDefinition)Definition).layerMask))
            {
                Debug.DrawLine(muzzle, hit.point, Color.red, 1.0f);
                
                var trail = ((FireAbilityDefinition)Definition).trailVisualEffect;
                var impact = ((FireAbilityDefinition)Definition).impactVisualEffect;

                if (impact)
                {
                    var data = new CueData { VectorData = new[] {hit.point, muzzle, hit.normal} };
                    Owner.PlayCue(impact, data, true);
                }

                if (trail)
                {
                    var data = new CueData { VectorData = new[] {hit.point, muzzle, hit.normal} };
                    Owner.PlayCue(trail, data, true);
                }
                
                var asc = hit.collider.GetComponent<AbilitySystemComponent>();
                if (asc) 
                {
                    asc.ExecuteEffect(((FireAbilityDefinition)Definition).damageEffect, Owner);
                }
            }

            // Always check for reload after shot, regardless of hit
            if (weaponSet.CurrentClip.CurrentValue <= 0)
            {
                if (Owner.IsLocalClient())
                {
                    Owner.AbilityManager.TryActivateAbility("ReloadWeaponAbility");
                }
            }

            EndAbility();
        }

        protected override void CancelAbility()
        {

        }

        public override void EndAbility()
        {

        }
    }
}
