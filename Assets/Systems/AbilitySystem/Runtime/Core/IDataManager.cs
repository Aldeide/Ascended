using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using GameplayTags.Runtime;
using JetBrains.Annotations;

namespace AbilitySystem.Runtime.Core
{
    public interface IDataManager
    {
        AbilityDefinition GetAbilityByName(string name);
        EffectDefinition GetEffectByName(string name);
        [CanBeNull] CueDefinition GetCueByTag(Tag tag);
        CueDefinition GetCueByTag(string tag);
    }
}
