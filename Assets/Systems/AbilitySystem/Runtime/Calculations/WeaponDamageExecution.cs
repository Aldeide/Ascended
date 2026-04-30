using System;
using AbilitySystem.Runtime.Calculations;
using AbilitySystem.Runtime.Effects;
using UnityEngine;

namespace AbilitySystem.Runtime.Calculations
{
    /// <summary>
    /// Example Execution Calculation that uses attributes from the Source (e.g. WeaponDamage, CritChance)
    /// and the Target (e.g. Armor) to calculate the final health reduction.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Calculations/Weapon Damage Execution", fileName = "WeaponDamageExecution")]
    public class WeaponDamageExecution : ExecutionCalculation
    {
        public override void Execute(Effect effect)
        {
            var source = effect.Source;
            var target = effect.Owner;

            if (source == null || target == null)
            {
                return;
            }

            // Retrieve source attributes (fallback to 0 if they don't exist in the system)
            var baseDamage = source.AttributeSetManager.GetAttribute("WeaponDamage")?.GetValue().CurrentValue ?? 10f;
            var critChance = source.AttributeSetManager.GetAttribute("CritChance")?.GetValue().CurrentValue ?? 0f;
            var critMultiplier = source.AttributeSetManager.GetAttribute("CritMultiplier")?.GetValue().CurrentValue ?? 1.5f;

            // Retrieve target attributes
            var armor = target.AttributeSetManager.GetAttribute("Armor")?.GetValue().CurrentValue ?? 0f;
            var healthAttr = target.AttributeSetManager.GetAttribute("Health");

            if (healthAttr == null)
            {
                return; // Target has no health attribute
            }

            // Simple damage formula
            float finalDamage = baseDamage;

            // Roll for Crit
            if (UnityEngine.Random.value <= critChance)
            {
                finalDamage *= critMultiplier;
            }

            // Simple armor mitigation (e.g. 100 armor = 50% damage reduction)
            float damageReduction = armor / (armor + 100f);
            finalDamage *= (1f - damageReduction);

            // Ensure we deal at least 1 damage
            finalDamage = Mathf.Max(1f, finalDamage);

            // Apply damage as an instant base value modification
            var newHealth = Mathf.Max(0f, healthAttr.BaseValue - finalDamage);
            healthAttr.SetBaseValue(newHealth);

            // Optional: You can play cues or emit events here based on crit!
            if (finalDamage > baseDamage && source.EventManager != null)
            {
                // source.EventManager.DispatchEvent(new GameplayEvent("Combat.CriticalHit"));
            }
        }
    }
}
