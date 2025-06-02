using AbilitySystem.Runtime.Abilities;
using Assets.ItemSystem.Runtime.Constants;
using Assets.ItemSystem.Runtime.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ItemSystem.Runtime.Definition
{
    public class EquipableDefinition : ItemDefinition, IEquippable
    {
        public List<EquipableSlot> Mods => throw new NotImplementedException();

        public Dictionary<ModType, int> MaxModByType => throw new NotImplementedException();

        public Ability EquippableAbility => throw new NotImplementedException();

        public void EquipItem()
        {
            throw new NotImplementedException();
        }

        public void UnequipItem()
        {
            throw new NotImplementedException();
        }
    }
}
