using AbilitySystem.Runtime.Abilities;

namespace ItemSystem.Runtime.Interface
{
    public interface IConsumable : IBaseItem
    {
        protected Ability ConsumableAbility { get; }

        Ability Consume();
    }
}
