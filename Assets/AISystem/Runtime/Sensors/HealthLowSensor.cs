using AbilitySystem.Scripts;
using AISystem.Runtime.WorldKeys;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace AISystem.Runtime.Sensors
{
    public class HealthLowSensor : LocalWorldSensorBase
    {
        public override void Created() {}

        public override void Update() {}

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            var asc = agent.Transform.GetComponent<AbilitySystemComponent>();
            if (asc == null || !asc.IsInitialized)
                return false;

            var health = asc.AbilitySystem.AttributeSetManager.GetAttribute("Health");
            var maxHealth = asc.AbilitySystem.AttributeSetManager.GetAttribute("MaxHealth");
            if (health == null || maxHealth == null)
                return false;

            // If health is below 30% of max health, mark as low.
            return (health.CurrentValue / maxHealth.CurrentValue) < 0.3f;
        }
    }
}
