using AbilitySystem.Runtime.Abilities;
using Assets.ItemSystem.Runtime.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ItemSystem.Runtime.Interface
{
    public interface IEquippableMod : IBaseItem
    {
        AbilityDefinition GetAbility();
        void DisableMod();
        void EnableMod();
        bool IsSlotableInto(EquippableSlot slot);
    }
}
