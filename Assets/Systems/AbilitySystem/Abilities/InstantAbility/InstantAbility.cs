using Systems.Abilities;
using Systems.AbilitySystem.Authoring.Abilities;
using Systems.AbilitySystem.Components;

namespace Systems.AbilitySystem.Abilities.InstantAbility
{
    public abstract class InstantAbilityT<T> : AbstractAbility<T> where T : InstantAbilityAssetBase
    {
        protected InstantAbilityT(T abilityAsset) : base(abilityAsset)
        {
        }
    }

    public abstract class InstantAbilitySpecT<T> : AbilitySpec<T> where T : AbstractAbility
    {
        protected InstantAbilitySpecT(T ability, AbilitySystemComponent owner) : base(ability, owner)
        {
        }
        
        public override void ActivateAbility(params object[] args)
        {
        }

        public override void CancelAbility()
        {
        }

        public override void EndAbility()
        {
        }

        protected override void AbilityTick()
        {
        }
    }
    
    public sealed class InstantAbility : InstantAbilityT<InstantAbilityAssetBase>
    {
        public InstantAbility(InstantAbilityAssetBase abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new InstantAbilitySpec(this, owner);
        }
    }
    
    public sealed class InstantAbilitySpec : InstantAbilitySpecT<InstantAbility>
    {
        public InstantAbilitySpec(InstantAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
        }
    }
}