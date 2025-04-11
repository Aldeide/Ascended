using Systems.AbilitySystem.Abilities;
using Systems.AbilitySystem.Authoring;
using Systems.AbilitySystem.Components;
using Unity.VisualScripting;

namespace Systems.Abilities
{
    public abstract class AbstractAbility
    {
        public readonly string Name;
        public readonly AbilityAsset Asset;
        public int Level;

        public AbstractAbility(AbilityAsset asset)
        {
            Asset = asset;
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