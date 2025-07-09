using AbilitySystem.Runtime.Abilities;

namespace ItemSystem.Runtime.Interface
{
    public interface IEquippableMod : IBaseItem
    {
        AbilityDefinition GetAbility();
        void DisableMod();
        void EnableMod();
        bool IsSlotableInto(EquippableSlot slot);
    }
}
