using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class DeathAbility : Ability
    {
        private float _respawnTimer = 5f;
        private float _currentTimer = 0;
        public DeathAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
        }

        protected override void ActivateAbility(AbilityData data)
        {
            _currentTimer = 0;
            Debug.Log("Death ability activated");
        }

        protected override void AbilityTick()
        {
            _currentTimer += Time.deltaTime;
            if (_currentTimer < _respawnTimer)
            {
                return;
            }

            Owner.Component.transform.position = new Vector3(0, 0, 0);
            Owner.Reset();
            TryEndAbility();
        }

        public override void EndAbility()
        {
            RemoveTags();
            Debug.Log("Death ability ended");
        }
    }
}