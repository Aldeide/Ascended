using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystemExtension.Runtime.AttributeSets;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Calculations
{
    [CreateAssetMenu(fileName = "ReloadCostMMC", menuName = "AbilitySystem/Calculations/ReloadCost")]
    public class ReloadCostMMC : ModifierMagnitudeCalculation
    {
        public override float CalculateMagnitude(Effect effect, float modifierMagnitude)
        {
            // For costs, source and target are usually the same (the owner of the ability)
            var target = effect.Owner;
            if (target == null) return 0;

            var weaponSet = target.AttributeSetManager.GetAttributeSet<WeaponAttributeSet>();
            if (weaponSet == null) return 0;

            float maxAmmo = weaponSet.ClipSize.CurrentValue;
            float currentAmmo = weaponSet.CurrentClip.CurrentValue;
            float baseReloadCost = weaponSet.ReloadEnergyCost.CurrentValue;

            // If modifierMagnitude is provided in the inspector, we can use it as a multiplier or override
            float multiplier = modifierMagnitude > 0 ? modifierMagnitude : 1.0f;

            // Calculate cost based on missing ammo
            float missingAmmo = Mathf.Max(0, maxAmmo - currentAmmo);
            
            // Return positive value because the modifier operation is Subtractive
            float finalCost = (missingAmmo / Mathf.Max(1, maxAmmo)) * baseReloadCost * multiplier;

            Debug.Log($"ReloadCostMMC: Missing {missingAmmo}/{maxAmmo} ammo. Calculated cost: {finalCost}");
            
            return finalCost;
        }
    }
}
