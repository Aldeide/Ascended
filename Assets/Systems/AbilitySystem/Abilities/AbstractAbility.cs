using Systems.AbilitySystem.Authoring;
using Systems.AbilitySystem.Components;

namespace Systems.AbilitySystem.Abilities
{
    public abstract class AbstractAbility
    {
        public readonly string Name;
        public readonly AbilityAsset Asset;
        public int Level;
        public AbilityTags AbilityTags;

        public AbstractAbility(AbilityAsset asset)
        {
            Asset = asset;
            Name = asset.uniqueName;
            AbilityTags = new AbilityTags(
                asset.AssetTags, asset.CancelAbilityTags, asset.BlockAbilityTags, asset.ActivationOwnedTags,
                asset.ActivationRequiredTags, asset.ActivationBlockedTags
            );
        }

        public abstract AbilitySpec CreateSpec(AbilitySystemComponent owner);
    }

    public abstract class AbstractAbility<T> : AbstractAbility where T : AbilityAsset
    {
        public T AbilityAsset => Asset as T;

        protected AbstractAbility(T abilityAsset) : base(abilityAsset)
        {
        }
    }
}