using System.Collections.Generic;
using System.Linq;
using GameplayTags.Generated;
using Sirenix.OdinInspector;

namespace GameplayTags.Runtime
{
    public static class TagsDropdown
    {
        private static ValueDropdownItem[] _gameplayTagChoices;

        public static IEnumerable<ValueDropdownItem> GameplayTagChoices
        {
            get
            {
                _gameplayTagChoices ??= TagLibrary.GetAllTags()
                    .Select(gameplayTag => new ValueDropdownItem(gameplayTag.Name, gameplayTag))
                    .ToArray();
                return _gameplayTagChoices;
            }
        }
        
        public static IEnumerable<ValueDropdownItem> FilteredGameplayTagChoices(string prefix)
        {
            return GameplayTagChoices.Where(v => v.Text.StartsWith(prefix));
        }
    }
}