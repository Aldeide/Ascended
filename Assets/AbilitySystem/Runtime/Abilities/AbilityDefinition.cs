using AbilitySystem.Scripts;

namespace AbilitySystem.Runtime.Abilities
{
    public abstract class AbilityDefinition
    {

            public readonly string Name;
            public readonly AbilityAsset Asset;
            public int Level;
            public AbilityTags AbilityTags;

            public AbilityDefinition(AbilityAsset asset)
            {
                Asset = asset;
                Name = asset.uniqueName;
                AbilityTags = new AbilityTags(
                    asset.AssetTags, asset.CancelAbilityTags, asset.BlockAbilityTags, asset.ActivationOwnedTags,
                    asset.ActivationRequiredTags, asset.ActivationBlockedTags
                );
            }

            public abstract Ability CreateSpec(AbilitySystemComponent owner);
        
    }
}