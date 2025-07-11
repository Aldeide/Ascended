using GameplayTags.Runtime;

namespace AbilitySystem.Runtime.Cues
{
    public interface ICueListener
    {
        public TagQuery TagQuery { get; set; }
    }
}