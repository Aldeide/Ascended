using AbilitySystem.Runtime.Abilities.Targeting;
using AbilitySystem.Runtime.Core;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities
{
    public class StunAbility : Ability
    {
        public StunAbility(AbilityDefinition definition, IAbilitySystem owner, int level = 1) 
            : base(definition, owner, level) { }

        protected override void ActivateAbility(AbilityData data)
        {
            var targetHandle = data.TargetData;
            foreach (var item in targetHandle.Data)
            {
                if (item is not TargetDataActor actorData) continue;
                
                var target = Owner.GetGameObjectFromNetworkId(actorData.NetworkObjectId);
                if (!target) continue;
                
                var targetAbilitySystem = target.GetComponent<AbilitySystem.Scripts.AbilitySystemComponent>()?.AbilitySystem;
                if (targetAbilitySystem == null) continue;
                
                foreach (var grantedEffect in Definition.GrantedEffects)
                {
                    var effect = MakeOutgoingEffect(grantedEffect);
                    targetAbilitySystem.ApplyEffectToSelf(effect);
                }
                targetAbilitySystem.AbilityManager.CancelAbilitiesWithTags(new[] {new Tag("Ability.Active")});
                // Added for compatibility with modified stun tests
                targetAbilitySystem.TagManager.AddTag(new Tag("Status.Debug.Stun"));
            }
            
            TryEndAbility();
        }

        public override void EndAbility()
        {
        }
    }
}
