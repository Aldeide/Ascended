using System.Collections.Generic;
using System.Linq;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ItemSystem.Runtime.Definition
{
    [CreateAssetMenu(fileName = "CurrencyDefinition", menuName = "EquipmentSystem/CurrencyDefinition")]
    public class CurrencyDefinition : ScriptableObject
    {
        [ValueDropdown("GetCurrencyDropdownValues", IsUniqueList = true, HideChildProperties = true)]
        public Tag CurrencyTag;
        
        private IEnumerable<ValueDropdownItem> GetCurrencyDropdownValues()
        {
            return TagsDropdown.GameplayTagChoices.Where(tag => tag.Text.StartsWith("Currency."));
        }
    }
}