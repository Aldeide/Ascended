
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using AbilitySystemExtension.Runtime.AttributeSets;
using AbilitySystemExtension.Runtime.Tags;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Systems.Movement
{
    public class PlayerMovementController : NetworkBehaviour
    {
        private Vector3 _movementInput = new Vector3(0, 0, 0);

        [SerializeField] private GameObject cameraTarget;
        
        private Animator _animator;
        private Rigidbody _rigidbody;
        private IAbilitySystem _abilitySystem;
        private float _movementSpeed;
        
        public float turnSmoothTime = 0.1f;   
        private float turnSmoothVelocity = 2.0f;
        private bool _isAiming;
        
        public override void OnNetworkSpawn()
        {
            
        }
        
        public void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _abilitySystem = GetComponent<AbilitySystemComponent>().AbilitySystem;
            _abilitySystem.AttributeSetManager.RegisterOnAttributeChanged("MovementSpeed", OnMovementSpeedChanged);
            _movementSpeed = _abilitySystem.AttributeSetManager.GetAttributeSet<CharacteristicsAttributeSet>()
                .MovementSpeed.CurrentValue;
        }

        public void Update()
        {
            if (!IsLocalPlayer) return;

            if (!CanMove()) return;

            _isAiming = _abilitySystem.TagManager.HasTag(TagLibrary.StatusAiming);
            
            
            if (_movementInput.magnitude <= 0.01f)
            {
                UpdateAnimator();
                return;
            }
            
            float targetAngle = Mathf.Atan2(_movementInput.x, _movementInput.z) * Mathf.Rad2Deg +
                                UnityEngine.Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            if (_isAiming)
            {
                Vector3 target = transform.position + UnityEngine.Camera.main.transform.forward;
                transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
            
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _rigidbody.MovePosition(this.transform.position += moveDirection * Time.deltaTime * _movementSpeed);
            UpdateAnimator();
        }
        
        private void Rotate(Vector3 newPosition)
        {
            Vector3 lookAtTarget = new Vector3(newPosition.x, this.transform.position.y, newPosition.z);
            Quaternion rotation = quaternion.LookRotation(lookAtTarget, Vector3.up);
            transform.LookAt(lookAtTarget);
        }

        private void UpdateAnimator()
        {
            if (_movementInput.magnitude > 0.01f)
            {
                _animator.SetBool("IsMoving", true);
                if (_isAiming)
                {
                    _animator.SetFloat("MovementX", _movementInput.x);
                    _animator.SetFloat("MovementY", _movementInput.z);
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
            if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Canceled)
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
            return !_abilitySystem.TagManager.HasAnyPartialTag(TagLibrary.StatusImmobilised);
        }
    }
}