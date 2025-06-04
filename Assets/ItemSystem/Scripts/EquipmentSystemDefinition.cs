using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Tags;
using Assets.ItemSystem.Runtime.Definition;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ItemSystem.Scripts
{
    [CreateAssetMenu(fileName = "EquipmentSystemDefinition", menuName = "EquipmentSystem/EquipmentSystemDefinition")]
    public class EquipmentSystemDefinition : ScriptableObject
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeSetsChoices", IsUniqueList = true)]
        public string[] attributeSets;

        public EquippableDefinition[] baseEquippable;
    }
}
