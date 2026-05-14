using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Abilities.Targeting;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Scripts;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class StunAbility : Ability
    {
        public StunAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }
        
        protected override void ActivateAbility(AbilityData data)
        {
            var targetData = data.TargetData;
            foreach (var target in targetData.Data)
            {
                var nId = ((TargetDataActor)target).NetworkObjectId;
                var targetGameObject = Owner.GetGameObjectFromNetworkId(nId);
                var asc = targetGameObject.GetComponent<AbilitySystemComponent>();
                if (!asc) return;
                var effectDefinition = Owner.DataManager.GetEffectByName("StunEffect");
                var effectContext = new EffectContext(Owner, Owner);
                var effect = Owner.MakeOutgoingEffect(effectDefinition, 1, effectContext);
                effect.Duration = 3;
                asc.AbilitySystem.EffectManager.AddEffect(effect);
            }
        }

        public override void EndAbility()
        {
            IsActive = false;
        }
    }
}