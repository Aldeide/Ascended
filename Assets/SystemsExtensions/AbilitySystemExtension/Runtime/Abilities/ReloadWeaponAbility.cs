using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AbilityTasks;
using AbilitySystem.Runtime.Core;
using AbilitySystemExtension.Runtime.AttributeSets;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class ReloadWeaponAbility : Ability
    {
        public ReloadWeaponAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        public override AbilityActivationResult CanActivate()
        {
            if (IsActive) return AbilityActivationResult.BlockedByAbility;

            var weaponSet = Owner.AttributeSetManager.GetAttributeSet<WeaponAttributeSet>();
            var charSet = Owner.AttributeSetManager.GetAttributeSet<CharacteristicsAttributeSet>();
            
            if (weaponSet == null || charSet == null) return AbilityActivationResult.MissingRequiredTag; // Using generic failure

            if (charSet.Energy.CurrentValue < weaponSet.ReloadEnergyCost.CurrentValue)
            {
                return AbilityActivationResult.CostFailed;
            }

            if (weaponSet.CurrentClip.CurrentValue >= weaponSet.ClipSize.CurrentValue)
            {
                return AbilityActivationResult.BlockedByAbility; // No need to reload
            }

            return base.CanActivate();
        }

        protected override void ActivateAbility(AbilityData data)
        {
            var weaponSet = Owner.AttributeSetManager.GetAttributeSet<WeaponAttributeSet>();
            var charSet = Owner.AttributeSetManager.GetAttributeSet<CharacteristicsAttributeSet>();

            CommitCostAndCooldown();
            float reloadTime = weaponSet.ReloadTime.CurrentValue;

            Debug.Log($"Reloading weapon... Time: {reloadTime}s");
            
            var waitTask = WaitDelayTask.CreateWaitDelay(this, reloadTime);
            waitTask.OnFinished += OnReloadFinished;
            waitTask.ReadyForActivation();
        }

        private void OnReloadFinished()
        {
            var weaponSet = Owner.AttributeSetManager.GetAttributeSet<WeaponAttributeSet>();
            // Refill clip base value
            weaponSet.CurrentClip.SetBaseValue(weaponSet.ClipSize.CurrentValue);

            
            TryEndAbility();
        }

        protected override void CancelAbility()
        {
            EndAbility();
        }

        public override void EndAbility()
        {
            IsActive = false;
        }
    }
}
