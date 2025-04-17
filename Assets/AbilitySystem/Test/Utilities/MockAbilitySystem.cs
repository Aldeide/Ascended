using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;

namespace AbilitySystem.Test.Utilities
{
    public class MockAbilitySystem : IAbilitySystem
    {
        GameplayTagManager IAbilitySystem.TagManager { get; set; }
        EffectManager IAbilitySystem.EffectManager { get; set; }
        AbilityManager IAbilitySystem.AbilityManager { get; set; }
        AttributeSetManager IAbilitySystem.AttributeSetManager { get; set; }
        public void Tick()
        {
            throw new System.NotImplementedException();
        }

        public float GetTime()
        {
            return 0;
        }

        public bool IsLocalClient()
        {
            return true;
        }
    }
}