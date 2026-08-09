using AbilitySystem.Scripts;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using AISystem.Runtime.DecisionMakers;
using UnityEngine;

namespace AISystem.Runtime.Sensors
{
    public class AllyNeedsHealingSensor : LocalWorldSensorBase
    {
        public override void Created() {}

        public override void Update() {}

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            // Bolt: O(1) lookup via ActiveInstances
            var components = AbilitySystemComponent.ActiveInstances;
            foreach (var comp in components)
            {
                if (comp == null || comp.gameObject == null || comp.gameObject == agent.Transform.gameObject) continue;
                
                // Only friendly AI agents
                if (!comp.CompareTag("Enemy") && comp.GetComponent<EnemyDecisionMaker>() == null)
                    continue;

                if (comp.IsInitialized)
                {
                    var health = comp.AbilitySystem.AttributeSetManager.GetAttribute("Health");
                    var maxHealth = comp.AbilitySystem.AttributeSetManager.GetAttribute("MaxHealth");
                    if (health != null && maxHealth != null && maxHealth.CurrentValue > 0f)
                    {
                        if ((health.CurrentValue / maxHealth.CurrentValue) < 0.5f)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
