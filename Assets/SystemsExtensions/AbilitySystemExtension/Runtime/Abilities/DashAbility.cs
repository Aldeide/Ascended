using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystemExtension.Scripts;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class DashAbility : ChargesAbility
    {
        private const float Duration = 0.3f;
        private const float Distance = 6;
        private Vector3 _startPosition = new Vector3();
        private Vector3 _endPosition = new Vector3();
        private float _startTime = 0;
        private readonly PlayerMovementController _playerMovementController;
        private readonly Rigidbody _rigidbody;
        private static readonly Tag DashingTag = new Tag("Status.Dashing");
        private static readonly int EnvironmentLayerMask = LayerMask.GetMask("Environment");
        
        public DashAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
            var ownerComponent = (UnityEngine.Component)Owner.NetworkRole;
            _playerMovementController = ownerComponent?.gameObject.GetComponent<PlayerMovementController>();
            _rigidbody = ownerComponent?.gameObject.GetComponent<Rigidbody>();
        }

        protected override void ActivateAbility(AbilityData data)
        {
            base.ActivateAbility(data);

            if (_rigidbody == null || _playerMovementController == null)
            {
                Debug.LogError($"DashAbility: Missing components! Rigidbody: {_rigidbody != null}, MovementController: {_playerMovementController != null}");
                TryEndAbility();
                return;
            }
            
            _startPosition = _rigidbody.position;
            Vector3 direction;
            if (_playerMovementController.MovementDirection.sqrMagnitude > 0.0001f /* sqrMagnitude is faster */)
            {
                direction = _playerMovementController.MovementDirection.normalized;
            }
            else
            {
                direction = ((UnityEngine.Component)Owner.NetworkRole).transform.forward.normalized;
            }
            
            _endPosition = _startPosition + direction * Distance;
            _startTime = Owner.GetTime();
            
            Owner.TagManager.AddTag(DashingTag);
        }

        protected override void AbilityTick()
        {
            float elapsed = Owner.GetTime() - _startTime;
            // Debug.Log($"Dash elapsed: {elapsed}, StartTime: {_startTime}, CurrentTime: {Owner.GetTime()}");
            
            if (elapsed >= Duration)
            {
                TryEndAbility();
                return;
            }

            float t = elapsed / Duration;
            // Target position based on the horizontal Lerp
            Vector3 targetPosHorizontal = Vector3.Lerp(_startPosition, _endPosition, t);
            
            Vector3 currentPos = _rigidbody.position;
            // We want to move from current to the next target position horizontally
            Vector3 moveDelta = targetPosHorizontal - currentPos;
            moveDelta.y = 0; // Keep it horizontal for the wall check
            
            // PERFORMANCE OPTIMIZATION:
            // Use sqrMagnitude to avoid an expensive Mathf.Sqrt() call for the early exit check.
            // When threshold is met, calculate Sqrt ONCE and cache it to compute normalization.
            // This avoids multiple implicit square roots from calling `.magnitude` and `.normalized`.
            float sqrMoveDist = moveDelta.sqrMagnitude;
            Vector3 nextPos = currentPos;

            if (sqrMoveDist > 0.000001f)
            {
                float moveDist = Mathf.Sqrt(sqrMoveDist);
                Vector3 moveDirection = moveDelta / moveDist;

                // Wall check at waist height (0.6m up) with a SphereCast.
                // This allows us to "see over" stairs and small obstacles (usually < 0.3m)
                // but still hit walls and large obstacles.
                Vector3 castOrigin = currentPos + Vector3.up * 0.6f;
                int environmentLayer = EnvironmentLayerMask;
                
                if (Physics.SphereCast(castOrigin, 0.3f, moveDirection, out var hit, moveDist, environmentLayer))
                {
                    // We hit a wall! Stop at the hit point.
                    nextPos = currentPos + moveDirection * Mathf.Max(0, hit.distance - 0.05f);
                    // Stop the dash progress if we hit a solid wall
                    _endPosition = nextPos;
                }
                else
                {
                    // Path is clear at waist height
                    nextPos = targetPosHorizontal;
                }
            }
            
            // Finally, handle the vertical adjustment for stairs and slopes
            if (Physics.Raycast(nextPos + Vector3.up * 2.0f, Vector3.down, out var groundHit, 4.0f, EnvironmentLayerMask))
            {
                nextPos.y = groundHit.point.y;
            }

            _rigidbody.MovePosition(nextPos);
        }

        public override void EndAbility()
        {
            Owner.TagManager.RemoveTag(DashingTag);
        }
    }
}
