using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace AbilitySystem.Runtime.Utilities
{
    public static class DropdownValuesUtil
    {
        private static ValueDropdownItem[] _gameplayTagChoices;
        public static IEnumerable<ValueDropdownItem> GameplayTagChoices
        {
            get
            {
                _gameplayTagChoices ??= ReflectionUtil.GameplayTags
                    .Select(gameplayTag => new ValueDropdownItem(gameplayTag.Name, gameplayTag))
                    .ToArray();
                return _gameplayTagChoices;
            }
        }
        
        public static IEnumerable<string> AttributeChoices => ReflectionUtil.AttributeNames;
        
        public static IEnumerable<string> AttributeSetsChoices => ReflectionUtil.AttributeSetsNames;
    }
}