using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemSystem.Runtime.Definition;
using UnityEngine;

namespace Assets.ItemSystem.Scripts
{
    [CreateAssetMenu(fileName = "EquipmentModSystemDefinition", menuName = "EquipmentSystem/EquipmentModSystemDefinition")]
    public class EquipmentModSystemDefinition : ScriptableObject
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeSetsChoices", IsUniqueList = true)]
        public string[] attributeSets;

        public EquippableModDefinition[] baseEquippableMods;
    }
}
