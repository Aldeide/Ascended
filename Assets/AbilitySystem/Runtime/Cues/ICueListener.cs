using AbilitySystem.Runtime.Tags;

namespace AbilitySystem.Runtime.Cues
{
    public interface ICueListener
    {
        public GameplayTag[] TagFilter { get; set; }
        
    }
}