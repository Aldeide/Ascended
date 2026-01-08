using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace AbilitySystem.Runtime.Utilities
{
    public static class DropdownValuesUtil
    {
        public static IEnumerable<string> AttributeChoices => ReflectionUtil.AttributeNames;
        
        public static IEnumerable<string> AttributeSetsChoices => ReflectionUtil.AttributeSetsNames;
    }
}