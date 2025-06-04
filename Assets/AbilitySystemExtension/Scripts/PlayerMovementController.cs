using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using AbilitySystemExtension.Runtime.AttributeSets;
using AbilitySystemExtension.Runtime.Tags;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystemExtension.Scripts
{
    public class PlayerMovementController : NetworkBehaviour
    {
        private static readonly Vector3 Offset = new Vector3(0, 0.1f, 0);
        private Vector3 _movementInput = new Vector3(0, 0, 0);

        [SerializeField] private GameObject cameraTarget;
        private CharacterController _characterController;
        private Animator _animator;
        private Rigidbody _rigidbody;
        private IAbilitySystem _abilitySystem;
        private IKCueListener _ikCueListener;
        private float _movementSpeed;
        [ShowInInspector] [SerializeField] private bool _isGrounded = true;
        public float turnSmoothTime = 0.1f;
        private float _turnSmoothVelocity = 0.2f;
        [ShowInInspector][SerializeField]
        private bool _isAiming;
        private Camera _camera;

        public Vector3 MovementDirection { get; private set; } = new Vector3(0, 0, 0);
        public Action<bool> OnGroundedChanged;

        public override void OnNetworkSpawn()
        {
        }

        public void Start()
        {
            _camera = Camera.main;
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _abilitySystem = GetComponent<AbilitySystemComponent>().AbilitySystem;
            _abilitySystem.AttributeSetManager.RegisterOnAttributeChanged("MovementSpeed", OnMovementSpeedChanged);
            _characterController = GetComponent<CharacterController>();
            _ikCueListener = GetComponent<IKCueListener>();
            _movementSpeed = _abilitySystem.AttributeSetManager.GetAttributeSet<CharacteristicsAttributeSet>()
                .MovementSpeed.CurrentValue;
        }

        public void Update()
        {
            // For now, movement is locally authoritative.
            if (!IsLocalPlayer) return;
            if (!CanMove()) return;
            
            // Update grounded state.
            UpdateGrounded();

            // Temp
            if (_isAiming)
            {
                _ikCueListener.EnableAimIK();
            }
            else
            {
                _ikCueListener.DisableAimIK();
            }
            
            var targetAngle = Mathf.Atan2(_movementInput.x, _movementInput.z) * Mathf.Rad2Deg +
                                _camera.transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity,
                turnSmoothTime);
            _rigidbody.MoveRotation(ComputeRotation(angle));
            ComputeMovementDirection(targetAngle);
            if (_movementInput.magnitude <= 0.01f)
            {
                UpdateAnimator();
                MovementDirection = Vector3.zero;
                return;
            }
            _rigidbody.MovePosition(transform.position += MovementDirection * (Time.deltaTime * _movementSpeed));
            UpdateAnimator();
        }

        public bool IsGrounded()
        {
            return _rigidbody.linearVelocity.y <= 0.01 && Physics.Raycast(transform.position + Offset, Vector3.down, 1f,
                LayerMask.GetMask("Environment"));
        }

        private void Rotate(Vector3 newPosition)
        {
            Vector3 lookAtTarget = new Vector3(newPosition.x, this.transform.position.y, newPosition.z);
            Quaternion rotation = Quaternion.LookRotation(lookAtTarget, Vector3.up);
            transform.LookAt(lookAtTarget);
        }

        private void UpdateAnimator()
        {
            if (_movementInput.magnitude > 0.01f)
            {
                _animator.SetBool("IsMoving", true);
                if (_isAiming)
                {
                    _animator.SetFloat("MovementX", _movementInput.x, 0.2f, Time.deltaTime);
                    _animator.SetFloat("MovementY", _movementInput.z, 0.2f, Time.deltaTime);
                }
                else
                {
                    _animator.SetFloat("MovementX", 0.0f);
                    _animator.SetFloat("MovementY", 1.0f);
                }
            }
            else
            {
                _animator.SetBool("IsMoving", false);
            }
        }

        public void OnMoveAction(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Performed or InputActionPhase.Canceled)
            {
                Vector2 input = context.ReadValue<Vector2>();
                _movementInput = new Vector3(input.x, 0, input.y);
            }
        }

        public void OnLookInput(InputAction.CallbackContext context)
        {
            //mousePosition = context.ReadValue<Vector2>();
        }

        public void OnMovementSpeedChanged(Attribute attribute, float oldValue, float newValue)
        {
            _movementSpeed = newValue;
        }

        public bool CanMove()
        {
            return !_abilitySystem.TagManager.HasAnyPartialTag(TagLibrary.StatusImmobilised) &&
                   !_abilitySystem.TagManager.HasAnyPartialTag(TagLibrary.StatusDead);
        }

        private void ComputeMovementDirection(float targetAngle)
        {
            if (_movementInput.magnitude <= 0.01f)
            {
                UpdateAnimator();
                MovementDirection = Vector3.zero;
                return;
            }
            
            MovementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        private Quaternion ComputeRotation(float angle)
        {
            if (!_isAiming) return Quaternion.Euler(0f, angle, 0f);
            var target = transform.position + _camera.transform.forward;
            var actualTarget = new Vector3(target.x, transform.position.y, target.z);
            return Quaternion.LookRotation(actualTarget - transform.position);
        }

        private void UpdateGrounded()
        {
            var previousGroundedState = _isGrounded;
            _isGrounded = IsGrounded();
            if (previousGroundedState != _isGrounded) OnGroundedChanged?.Invoke(_isGrounded);
        }
    }
}