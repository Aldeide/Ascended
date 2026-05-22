using AbilitySystem.Scripts;
using AbilitySystem.Runtime.Abilities;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace AISystem.Runtime.Actions
{
    public class GoapAbilityAction : GoapActionBase<GoapAbilityAction.Data, GoapAbilityAction.PropertiesClass>
    {
        public class Data : IActionData
        {
            public ITarget Target { get; set; }
            public float Timer { get; set; }
            public bool AbilityTriggered { get; set; }
        }

        public class PropertiesClass : IActionProperties
        {
            public string AbilityName { get; set; }
        }

        public override void Start(IMonoAgent agent, Data data)
        {
            data.Timer = 0f;
            data.AbilityTriggered = false;
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            var asc = agent.Transform.GetComponent<AbilitySystemComponent>();
            if (asc == null || !asc.IsInitialized)
            {
                return ActionRunState.Stop;
            }

            var abilityName = Properties?.AbilityName;
            if (string.IsNullOrEmpty(abilityName))
            {
                return ActionRunState.Stop;
            }

            if (!data.AbilityTriggered)
            {
                if (!asc.AbilitySystem.AbilityManager.Abilities.TryGetValue(abilityName, out var abilityInstance))
                {
                    return ActionRunState.Stop;
                }

                // Verify CanActivate
                if (abilityInstance.CanActivate() != AbilityActivationResult.Success)
                {
                    // If not ready or on cooldown, we stop the action so GOAP replans
                    return ActionRunState.Stop;
                }

                var abilityData = new AbilityData();
                if (data.Target != null)
                {
                    abilityData.TargetPosition = data.Target.Position;
                    if (data.Target is TransformTarget transformTarget && transformTarget.Transform != null)
                    {
                        var networkObject = transformTarget.Transform.GetComponent<Unity.Netcode.NetworkObject>();
                        if (networkObject != null)
                        {
                            var actorData = new AbilitySystem.Runtime.Abilities.Targeting.TargetDataActor 
                            { 
                                NetworkObjectId = networkObject.NetworkObjectId 
                            };
                            abilityData.TargetData.Add(actorData);
                        }
                    }
                }

                asc.TryActivateAbility(abilityName, abilityData);
                data.AbilityTriggered = true;

                if (!abilityInstance.IsActive)
                {
                    return ActionRunState.Completed;
                }
            }
            else
            {
                if (!asc.AbilitySystem.AbilityManager.Abilities.TryGetValue(abilityName, out var abilityInstance) || !abilityInstance.IsActive)
                {
                    return ActionRunState.Completed;
                }
            }

            return ActionRunState.Continue;
        }
    }
}
