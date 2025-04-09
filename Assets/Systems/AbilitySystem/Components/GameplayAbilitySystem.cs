using System.Collections.Generic;
using FishNet.Object;

namespace Systems.AbilitySystem.Components
{
    public class GameplayAbilitySystem : NetworkBehaviour
    {
        private List<AbilitySystemComponent> abilitySystemComponents = new();

        public void Update()
        {
            if (!IsServerStarted) return;

            foreach (var asc in abilitySystemComponents)
            {
                asc.Tick();
            }
        }
    }
}