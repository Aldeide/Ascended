using AbilitySystem.Runtime.Cues;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public static class CueUtilities
    {
        public static CueDefinition CreateCueDefinitionWithTag(string cueTag)
        {
            var tag = new Tag(cueTag);
            var cueDef = ScriptableObject.CreateInstance<CueDefinition>();
            cueDef.CueTag = tag;
            return cueDef;
        }
        
        public static CueDefinition CreateCueDefinitionWithTag(Tag cueTag)
        {
            var cueDef = ScriptableObject.CreateInstance<CueDefinition>();
            cueDef.CueTag = cueTag;
            return cueDef;
        }
    }
}