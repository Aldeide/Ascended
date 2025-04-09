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
                Debug.LogError("[EX] Type 'GTagLib' not found. Please generate the TAGS CODE first!");
                return Array.Empty<GameplayTag>();
            }

            const string fieldName = "TagMap";
            var field = tagLibType.GetField("TagMap", BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError($"[EX] Field \"{fieldName}\" not found in GTagLib!");
                return Array.Empty<GameplayTag>();
            }

            var value = field.GetValue(null);
            if (value is not Dictionary<string, GameplayTag> tagMap)
            {
                Debug.LogError($"[EX] Field \"{fieldName}\" is not a Dictionary<string, GameplayTag> in GTagLib!");
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
                Debug.LogError("[EX] Type 'GAttrSetLib' not found. Please generate the GAttrSetLib CODE first!");
                return Array.Empty<string>();
            }

            const string fieldName = "AttributeFullNames";
            var field = libType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError($"[EX] Field \"{fieldName}\" not found in GAttrSetLib!");
                return Array.Empty<string>();
            }

            var value = field.GetValue(null);
            if (value is not List<string> list)
            {
                Debug.LogError($"[EX] Field \"{fieldName}\" is not a List<string> in GAttrSetLib!");
                return Array.Empty<string>();
            }

            return list.ToArray();
        }
        
    }
}