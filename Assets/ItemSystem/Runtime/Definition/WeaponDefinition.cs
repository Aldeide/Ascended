using AbilitySystem.Runtime.Abilities;
using Assets.ItemSystem.Runtime.Constants;
using Assets.ItemSystem.Runtime.Interface;
using ItemSystem.Runtime.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ItemSystem.Runtime.Definition
{
    public class WeaponDefinition : EquipableDefinition, IWeapon
    {
        WeaponType IWeapon.Type => throw new NotImplementedException();
    }
}
