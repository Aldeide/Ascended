using Assets.ItemSystem.Scripts;
using ItemSystem.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ItemSystem.Runtime.Interface.Core
{
    public interface IEquippableSystemManager
    {
        public EquipmentComponent Component { get; set; }
    }
}
