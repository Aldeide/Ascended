using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using AbilitySystemExtension.Runtime.AttributeSets;
using GameplayTags.Generated;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using Systems.Animation;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;
using Item.Scripts.UI;

namespace AbilitySystemExtension.Scripts
{
    [RequireComponent(typeof(AnimationController))]
    public class PlayerMovementController : NetworkBehaviour
    {
        // Offset for grounded checks.
        private static readonly Vector3 Offset = new Vector3(0, 0.1f, 0);
        private static readonly Tag DashingTag = new Tag("Status.Dashing");
        private static readonly Tag StunnedTag = new Tag("Status.Debuff.Stun");
        private static int _environmentLayerMask = 0;
        [ShowInInspector] [SerializeField] private Vector3 _movementInput = new Vector3(0, 0, 0);

        [FormerlySerializedAs("cameraTarget")] [SerializeField]
        private GameObject CameraTarget;

        private AnimationController _animationController;
        private Rigidbody _rigidbody;
        private IAbilitySystem _abilitySystem;
        private IKCueListener _ikCueListener;
        private float _movementSpeed;
        [ShowInInspector] [SerializeField] private bool _isGrounded = true;
        public float turnSmoothTime = 0.1f;
        private float _turnSmoothVelocity = 0.2f;
        [ShowInInspector] [SerializeField] private bool _manualAiming;
        [ShowInInspector] [SerializeField] private bool _combatStance;
        private Camera _camera;

        public Vector3 MovementDirection { get; private set; } = new Vector3(0, 0, 0);
        public Action<bool> OnGroundedChanged;

        private InventoryUIController _inventoryUI;

        private InventoryUIController GetInventoryUI()
        {
            if (_inventoryUI == null)
            {
                _inventoryUI = GetComponentInChildren<InventoryUIController>();
            }
            return _inventoryUI;
        }

        public override void OnNetworkSpawn()
        {
        }

        private void Awake()
        {
            _animationController = GetComponent<AnimationController>();
            _rigidbody = GetComponent<Rigidbody>();
            _ikCueListener = GetComponent<IKCueListener>();
            _environmentLayerMask = LayerMask.GetMask("Environment");
        }

        public void Start()
        {
            _camera = Camera.main;
            _abilitySystem = GetComponent<AbilitySystemComponent>().AbilitySystem;
            _abilitySystem.AttributeSetManager.RegisterOnAttributeChanged("MovementSpeed", OnMovementSpeedChanged);
            _movementSpeed = _abilitySystem.AttributeSetManager.GetAttributeSet<CharacteristicsAttributeSet>()
                .MovementSpeed.CurrentValue;
        }

        public void Update()
        {
            // For now, movement is locally authoritative.
            if (!IsLocalPlayer) return;

            if (_camera == null)
            {
                _camera = Camera.main;
            }

            var inv = GetInventoryUI();
            if (inv != null && inv.IsMenuOpen)
            {
                // Smoothly rotate character to face the camera
                if (_camera != null)
                {
                    Vector3 dirToCamera = _camera.transform.position - transform.position;
                    dirToCamera.y = 0f; // Keep rotation horizontal
                    if (dirToCamera.sqrMagnitude > 0.001f)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(dirToCamera);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
                    }
                }

                // Stop movement and update animator
                _movementInput = Vector3.zero;
                MovementDirection = Vector3.zero;
                UpdateAnimator();
                return;
            }

            if (!CanMove())
            {
                MovementDirection = Vector3.zero;
                UpdateAnimator();
                return;
            }

            // Update grounded state.
            UpdateGrounded();

            // Temp
            if (_ikCueListener != null)
            {
                if (IsInAimingState())
                {
                    _ikCueListener.EnableAimIK();
                }
                else
                {
                    _ikCueListener.DisableAimIK();
                }
            }

            var targetAngle = Mathf.Atan2(_movementInput.x, _movementInput.z) * Mathf.Rad2Deg +
                              _camera.transform.eulerAngles.y;
            _targetAngle = targetAngle; // Store for FixedUpdate
            
            _currentAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity,
                turnSmoothTime);

            ComputeMovementDirection(targetAngle);
            UpdateAnimator();
        }

        private float _targetAngle;
        private float _currentAngle;

        public void FixedUpdate()
        {
            if (!IsLocalPlayer) return;
            if (!CanMove()) return;

            if (IsInAimingState() || _movementInput.sqrMagnitude > 0.0001f /* sqrMagnitude is faster */)
            {
                _rigidbody.MoveRotation(ComputeRotation(_currentAngle));
            }

            if (_movementInput.sqrMagnitude > 0.0001f /* sqrMagnitude is faster */)
            {
                _rigidbody.MovePosition(_rigidbody.position + MovementDirection * (Time.fixedDeltaTime * _movementSpeed));
            }
        }

        public bool IsGrounded()
        {
            return _rigidbody.linearVelocity.y <= 0.01 && Physics.Raycast(transform.position + Offset, Vector3.down, 1f,
                _environmentLayerMask);
        }

        private void Rotate(Vector3 newPosition)
        {
            Vector3 lookAtTarget = new Vector3(newPosition.x, this.transform.position.y, newPosition.z);
            Quaternion rotation = Quaternion.LookRotation(lookAtTarget, Vector3.up);
            transform.LookAt(lookAtTarget);
        }

        private void UpdateAnimator()
        {
            if (CanMove() && _movementInput.sqrMagnitude > 0.0001f /* sqrMagnitude is faster */)
            {
                _animationController.SetIsMoving(true);
                if (IsInAimingState())
                {
                    _animationController.SetMovement(_movementInput.x, _movementInput.z, 0.2f, Time.deltaTime);
                }
                else
                {
                    _animationController.SetMovement(0, 1);
                }
            }
            else
            {
                _animationController.SetIsMoving(false);
            }

            _animationController.SetIsFiring(IsInAimingState());
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
            var inv = GetInventoryUI();
            if (inv != null && inv.IsMenuOpen) return false;

            return !_abilitySystem.TagManager.HasAnyPartialTag(TagLibrary.Status.Immobilised) &&
                   !_abilitySystem.TagManager.HasAnyPartialTag(TagLibrary.Status.Debuff.Stun) &&
                   !_abilitySystem.TagManager.HasAnyPartialTag(TagLibrary.Status.Dead) &&
                   !_abilitySystem.TagManager.HasTag(DashingTag);
        }

        private void ComputeMovementDirection(float targetAngle)
        {
            if (_movementInput.sqrMagnitude <= 0.0001f /* sqrMagnitude is faster */)
            {
                UpdateAnimator();
                MovementDirection = Vector3.zero;
                return;
            }

            MovementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        private Quaternion ComputeRotation(float angle)
        {
            if (!IsInAimingState() || _camera == null) return Quaternion.Euler(0f, angle, 0f);
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

        public void SetManualAiming(bool manualAiming)
        {
            _manualAiming = manualAiming;
        }

        public void SetCombatStance(bool combatStance)
        {
            _combatStance = combatStance;
        }

        private bool IsInAimingState()
        {
            return _manualAiming || _combatStance;
        }
    }
}