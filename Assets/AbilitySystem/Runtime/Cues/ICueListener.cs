using AbilitySystem.Runtime.Tags;

namespace AbilitySystem.Runtime.Cues
{
    public interface ICueListener
    {
        public GameplayTagQuery TagQuery { get; set; }
    }
}