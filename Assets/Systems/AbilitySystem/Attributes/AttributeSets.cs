using System;
using System.Collections.Generic;

namespace Systems.AbilitySystem.Attributes
{
    public static class AttributeSets
    {
        public static Dictionary<Type, string> AttrSetNameCache { get; private set; }
        
        public static void Cache(Dictionary<Type,string> typeToName)
        {
            AttrSetNameCache = typeToName;
        }

        public static string AttributeSetName(Type attrSetType)
        {
            if(AttrSetNameCache==null)
                return attrSetType.Name;
            
            return AttrSetNameCache.TryGetValue(attrSetType, out var value) ? value : attrSetType.Name;
        }
    }
}