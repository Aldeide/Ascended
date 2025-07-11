using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AbilitySystem.Runtime.Tags;
using UnityEngine;

namespace AbilitySystem.Runtime.Utilities
{
    public static class ReflectionUtil
    {
        private static string[] _attributeNames;
        private static string[] _attributeSetNames;
        
        public static IEnumerable<string> AttributeNames
        {
            get
            {
                _attributeNames ??= LoadAttributeNames();
                return _attributeNames;
            }
        }

        private static string[] LoadAttributeNames()
        {
            var libType = TypeUtil.FindTypeInAllAssemblies("AbilitySystemExtension.Runtime.AttributeSets.AttributeSetLibrary");
            if (libType == null)
            {
                Debug.LogError("AttributeSetLibrary not found!");
                return Array.Empty<string>();
            }

            const string fieldName = "AttributeFullNames";
            var field = libType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError($"Field \"{fieldName}\" not found in AttributeSetLibrary!");
                return Array.Empty<string>();
            }

            var value = field.GetValue(null);
            if (value is not List<string> list)
            {
                Debug.LogError($"Field \"{fieldName}\" is not a List<string> in AttributeSetLibrary!");
                return Array.Empty<string>();
            }

            return list.ToArray();
        }

        private static string[] _attributeSetsNames;

        public static IEnumerable<string> AttributeSetsNames
        {
            get
            {
                _attributeSetNames ??= LoadAttributeSetNames();
                return _attributeSetNames;
            }
        }

        private static string[] LoadAttributeSetNames()
        {
            var libType = TypeUtil.FindTypeInAllAssemblies("AbilitySystemExtension.Runtime.AttributeSets.AttributeSetLibrary");
            if (libType == null)
            {
                Debug.LogError("AttributeSetLibrary not found!");
                return Array.Empty<string>();
            }
            const string fieldName = "AttributeSetTypeDict";
            var field = libType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError($"Field \"{fieldName}\" not found in AttributeSetLibrary!");
                return Array.Empty<string>();
            }

            var value = field.GetValue(null);
            if (value is not Dictionary<string, Type> dict)
            {
                Debug.LogError($"Field \"{fieldName}\" is not a Dictionary<string, Type> in AttributeSetLibrary!");
                return Array.Empty<string>();
            }

            return dict.Keys.ToArray();
        }

        public static Type GetAttributeSetType(string name)
        {
            var libType = TypeUtil.FindTypeInAllAssemblies("AbilitySystemExtension.Runtime.AttributeSets.AttributeSetLibrary");
            if (libType == null)
            {
                Debug.LogError("AttributeSetLibrary not found!");
                return null;
            }
            const string fieldName = "AttributeSetTypeDict";
            var field = libType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError($"Field \"{fieldName}\" not found in AttributeSetLibrary!");
                return null;
            }

            var value = field.GetValue(null);
            if (value is not Dictionary<string, Type> dict)
            {
                Debug.LogError($"Field \"{fieldName}\" is not a Dictionary<string, Type> in AttributeSetLibrary!");
                return null;
            }

            return dict[name];
        }
        
    }
}