using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;

namespace AbilitySystem.Runtime.Core
{
    public interface IAbilitySystem
    {
        public GameplayTagManager TagManager { get; protected set; }
        public EffectManager EffectManager { get; protected set; }
        public AbilityManager AbilityManager { get; protected set; }

        public void Tick();
        
        public bool IsLocalClient();
    }
}