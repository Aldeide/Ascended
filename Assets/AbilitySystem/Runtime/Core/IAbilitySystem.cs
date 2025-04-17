using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;

namespace AbilitySystem.Runtime.Core
{
    public interface IAbilitySystem
    {
        public GameplayTagManager TagManager { get; set; }
        public EffectManager EffectManager { get; set; }
        public AbilityManager AbilityManager { get; set; }
        public AttributeSetManager AttributeSetManager { get; set; }

        public void Tick();

        public float GetTime();
        
        public bool IsLocalClient();
    }
}