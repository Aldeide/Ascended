using AbilitySystem.Scripts;
using AbilitySystem.Runtime.Abilities;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace AISystem.Runtime.Sensors
{
    public class AbilitySensor : LocalWorldSensorBase
    {
        public string AbilityName { get; set; }
        public bool CheckReady { get; set; } = true;

        public override void Created() {}

        public override void Update() {}

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            if (string.IsNullOrEmpty(AbilityName))
                return false;

            var asc = agent.Transform.GetComponent<AbilitySystemComponent>();
            if (asc == null || !asc.IsInitialized)
                return false;

            if (!asc.AbilitySystem.AbilityManager.Abilities.TryGetValue(AbilityName, out var abilityInstance))
            {
                return false;
            }

            if (CheckReady)
            {
                return abilityInstance.CanActivate() == AbilityActivationResult.Success;
            }

            // Otherwise, we just check if it is present.
            return true;
        }
    }
}
