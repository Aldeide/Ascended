using Assets.ItemSystem.Runtime.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ItemSystem.Runtime.Definition
{
    [Serializable]
    public class ItemDefinition : ScriptableObject, IBaseItem
    {
        public string Name => throw new NotImplementedException();
    }
}
