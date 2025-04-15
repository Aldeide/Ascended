using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;

namespace AbilitySystem.Test.TestUtils
{
    public class MockAbilitySystem : IAbilitySystem
    {
        GameplayTagManager IAbilitySystem.TagManager { get; set; }
        EffectManager IAbilitySystem.EffectManager { get; set; }
        AbilityManager IAbilitySystem.AbilityManager { get; set; }

        public void Tick()
        {
            throw new System.NotImplementedException();
        }

        public bool IsLocalClient()
        {
            return true;
        }
    }
}