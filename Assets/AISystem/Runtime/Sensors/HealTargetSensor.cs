using AbilitySystem.Scripts;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using AISystem.Runtime.DecisionMakers;
using UnityEngine;

namespace AISystem.Runtime.Sensors
{
    public class HealTargetSensor : LocalTargetSensorBase
    {
        public override void Created() {}

        public override void Update() {}

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var components = AbilitySystemComponent.Instances;
            AbilitySystemComponent lowestAlly = null;
            float lowestRatio = 1.0f;

            foreach (var comp in components)
            {
                if (comp.gameObject == agent.Transform.gameObject) continue;
                
                // Only friendly AI agents
                if (!comp.CompareTag("Enemy") && comp.GetComponent<EnemyDecisionMaker>() == null)
                    continue;

                if (comp.IsInitialized)
                {
                    var health = comp.AbilitySystem.AttributeSetManager.GetAttribute("Health");
                    var maxHealth = comp.AbilitySystem.AttributeSetManager.GetAttribute("MaxHealth");
                    if (health != null && maxHealth != null && maxHealth.CurrentValue > 0f)
                    {
                        float ratio = health.CurrentValue / maxHealth.CurrentValue;
                        if (ratio < 0.5f && ratio < lowestRatio)
                        {
                            lowestRatio = ratio;
                            lowestAlly = comp;
                        }
                    }
                }
            }

            if (lowestAlly != null)
            {
                return new TransformTarget(lowestAlly.transform);
            }

            return null;
        }
    }
}
