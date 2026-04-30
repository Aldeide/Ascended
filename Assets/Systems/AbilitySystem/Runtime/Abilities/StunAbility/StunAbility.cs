using AbilitySystem.Runtime.Abilities.Targeting;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.StunAbility
{
    public class StunAbility : Ability
    {
        public StunAbility(AbilityDefinition definition, IAbilitySystem owner, int level = 1) 
            : base(definition, owner, level) { }

        protected override void ActivateAbility(AbilityData data)
        {
            // Extract targets from TargetData
            // In a real scenario, we would resolve the NetworkObjectId to a GameObject.
            // For now, we assume the TargetData contains information that the Ability can use.
            
            var targetHandle = data.TargetData;
            foreach (var item in targetHandle.Data)
            {
                if (item is TargetDataActor actorData)
                {
                    // Resolve GameObject from NetworkObjectId
                    // For the sake of this implementation and tests, we'll use a simplified resolution
                    // In a full Unity project, this would use NetworkManager.Singleton.SpawnManager
                    GameObject target = Owner.GetGameObjectFromNetworkId(actorData.NetworkObjectId);
                    
                    if (target != null)
                    {
                        var targetAbilitySystem = target.GetComponent<IAbilitySystem>();
                        if (targetAbilitySystem != null)
                        {
                            foreach (var grantedEffect in Definition.GrantedEffects)
                            {
                                var effect = MakeOutgoingEffect(grantedEffect);
                                targetAbilitySystem.ApplyEffectToSelf(effect);
                            }
                        }
                    }
                }
            }
            
            TryEndAbility();
        }

        public override void EndAbility()
        {
        }
    }
}
