using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Systems.AbilitySystem.Tags;
using UnityEngine;

namespace Systems.AbilitySystem.Util
{
    public static class ReflectionUtil
    {
        private static GameplayTag[] _tags;
        private static string[] _attributeNames;
        private static string[] _attributeSetNames;
        public static IEnumerable<GameplayTag> GameplayTags
        {
            get
            {
                _tags ??= LoadTags();
                return _tags;
            }
        }

        private static GameplayTag[] LoadTags()
        {
            var tagLibType = TypeUtil.FindTypeInAllAssemblies("Authoring.Tags.TagLibrary");
            if (tagLibType == null)
            {
                Debug.LogError("TagLibrary not found!");
                return Array.Empty<GameplayTag>();
            }

            const string fieldName = "TagMap";
            var field = tagLibType.GetField("TagMap", BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError($"Field \"{fieldName}\" not found in TagLibrary!");
                return Array.Empty<GameplayTag>();
            }

            var value = field.GetValue(null);
            if (value is not Dictionary<string, GameplayTag> tagMap)
            {
                Debug.LogError($"Field \"{fieldName}\" is not a Dictionary<string, GameplayTag> in TagLibrary!");
                return Array.Empty<GameplayTag>();
            }

            return tagMap.Values.ToArray();
        }
        
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
            var libType = TypeUtil.FindTypeInAllAssemblies("Authoring.AttributeSets.AttributeSetLibrary");
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
        
    }
}