using AbilitySystem.Runtime.Abilities;

namespace Item.Runtime.Interface
{
    public interface IConsumable : IBaseItem
    {
        protected Ability ConsumableAbility { get; }

        Ability Consume();
    }
}
