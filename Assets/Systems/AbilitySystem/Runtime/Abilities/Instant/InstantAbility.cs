using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;

namespace AbilitySystem.Runtime.Abilities
{
    public class InstantAbility : Ability
    {
        public InstantAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        public override bool TryActivateAbility(PredictionKey key, AbilityData data, bool force = false)
        {
            return base.TryActivateAbility(key, data, force);
        }

        protected override void ActivateAbility(AbilityData data)
        {
            
        }

        public override void EndAbility()
        {
            
        }
    }
}