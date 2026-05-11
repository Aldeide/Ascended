using AbilitySystem.Runtime.Calculations;
using AbilitySystem.Runtime.Effects;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    /// <summary>
    /// Example ExecutionCalculation that handles complex stun logic.
    /// In this example, it could check for resistance or modify the effect based on attributes.
    /// </summary>
    [CreateAssetMenu(fileName = "StunExecution", menuName = "AbilitySystem/Executions/Stun")]
    public class StunExecutionCalculation : ExecutionCalculation
    {
        public override void Execute(Effect effect)
        {
            // Example logic:
            // 1. Get source and target systems
            var source = effect.Source;
            var target = effect.Owner;

            // 2. We could calculate a chance to stun based on attributes here
            // e.g. float stunChance = source.AttributeSetManager.GetAttributeValue("StunChance");
            
            // 3. For this example, we'll just ensure the stun effect is applied
            // (The stun effect already applies tags via GrantedTags, but we could add more here)
        }
    }
}
