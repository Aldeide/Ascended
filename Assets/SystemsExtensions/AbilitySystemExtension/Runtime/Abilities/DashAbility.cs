using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystemExtension.Scripts;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class DashAbility : Ability
    {
        private const float Duration = 0.3f;
        private const float Distance = 4;
        private Vector3 _startPosition = new Vector3();
        private Vector3 _endPosition = new Vector3();
        private float _startTime = 0;
        private readonly PlayerMovementController _playerMovementController;
        
        public DashAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
            _playerMovementController = Owner.Component.gameObject.GetComponent<PlayerMovementController>();
        }

        protected override void ActivateAbility(AbilityData data)
        {
            _startPosition = Owner.Component.transform.position;
            if (_playerMovementController.MovementDirection.magnitude > 0.01f)
            {
                _endPosition = _startPosition + _playerMovementController.MovementDirection.normalized * Distance;
            }
            else
            {
                _endPosition = _startPosition + Owner.Component.transform.forward.normalized * Distance;
            }
            
            _startTime = Owner.GetTime();
            CommitCostAndCooldown();
        }

        protected override void AbilityTick()
        {
            if (Owner.GetTime() - _startTime >= Duration)
            {
                TryEndAbility();
                return;
            }

            Owner.Component.transform.position =
                Vector3.Lerp(_startPosition, _endPosition, (Owner.GetTime() - _startTime) / Duration);
        }

        public override void EndAbility()
        {
        }
    }
}