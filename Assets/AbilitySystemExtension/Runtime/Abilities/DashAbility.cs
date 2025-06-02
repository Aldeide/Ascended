using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystemExtension.Scripts;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class DashAbility : Ability
    {
        private readonly float _duration = 0.3f;
        private readonly float _distance = 4;
        private Vector3 _startPosition = new Vector3();
        private Vector3 _endPosition = new Vector3();
        private float _startTime = 0;
        private PlayerMovementController _playerMovementController;
        
        public DashAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
            _playerMovementController = Owner.Component.gameObject.GetComponent<PlayerMovementController>();
        }

        protected override void ActivateAbility(AbilityData data)
        {
            _startPosition = Owner.Component.transform.position;
            _endPosition = _startPosition + _playerMovementController.MovementDirection * _distance;
            _startTime = Owner.GetTime();
            CommitCostAndCooldown();
        }

        protected override void AbilityTick()
        {
            if (Owner.GetTime() - _startTime >= _duration)
            {
                TryEndAbility();
                return;
            }

            Owner.Component.transform.position =
                Vector3.Lerp(_startPosition, _endPosition, (Owner.GetTime() - _startTime) / _duration);
        }

        protected override void CancelAbility()
        {
            base.CancelAbility();
        }

        public override void EndAbility()
        {
        }
    }
}