using AbilitySystem.Runtime.Core;

namespace AbilitySystem.Runtime.Effects
{
    public class EffectManager
    {
        private IAbilitySystem _owner;
        
        public EffectManager(IAbilitySystem owner)
        {
            _owner = owner;
        }
    }
}