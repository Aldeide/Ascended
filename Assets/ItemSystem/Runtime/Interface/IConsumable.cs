using AbilitySystem.Runtime.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ItemSystem.Runtime.Interface
{
    public interface IConsumable : IBaseItem
    {
        protected Ability ConsumableAbility { get; }

        Ability Consume();
    }
}
