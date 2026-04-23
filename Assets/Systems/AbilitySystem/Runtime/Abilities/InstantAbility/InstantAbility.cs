using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;

namespace AbilitySystem.Runtime.Abilities.InstantAbility
{
    public class InstantAbility : Ability
    {
        public InstantAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        public override bool TryActivateAbility(PredictionKey key, AbilityData data)
        {
            return base.TryActivateAbility(key, data);
        }

        protected override void ActivateAbility(AbilityData data)
        {
            
        }

        public override void EndAbility()
        {
            
        }
    }
}