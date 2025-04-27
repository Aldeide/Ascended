using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class FireAbility : Ability
    {
        public FireAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        public override void ActivateAbility(params object[] args)
        {
            var target = args[1];
            var impact = ((FireAbilityDefinition)Definition).impactVisualEffect;
            PlayActivationCues();
            if (impact)
            {
                CueData data = new CueData();
                data.position = (Vector3)target;
                Owner.PlayCue(impact, data);
            }

        }

        public override void CancelAbility()
        {

        }

        public override void EndAbility()
        {

        }
    }
}