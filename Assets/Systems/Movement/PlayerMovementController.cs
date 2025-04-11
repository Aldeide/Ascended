using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Systems.Movement
{
    public class PlayerMovementController : MonoBehaviour
    {
        private Vector3 movementInput = new Vector3(0, 0, 0);
        private Vector2 mousePosition = new Vector2(0, 0);
        private Vector3 movement = new Vector3(0, 0, 0);
        [SerializeField] private GameObject cameraTarget;
        
        private Animator _animator;
        private Rigidbody _rigidbody;
        
        public float turnSmoothTime = 0.1f;   
        private float turnSmoothVelocity = 2.0f;
        
        public void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Update()
        {
            if (movementInput.magnitude <= 0.01f)
            {
                UpdateAnimator();
                return;
            }


            float targetAngle = Mathf.Atan2(movementInput.x, movementInput.z) * Mathf.Rad2Deg +
                                UnityEngine.Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //cameraTarget.transform.Rotate(new Vector3(0f, -angle, 0f));
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            _rigidbody.MovePosition(this.transform.position += moveDirection * Time.deltaTime * 2.0f);
            //Rotate(this.transform.position + movement);
            UpdateAnimator();
        }
        
        private void Rotate(Vector3 newPosition)
        {
            Vector3 lookAtTarget = new Vector3(newPosition.x, this.transform.position.y, newPosition.z);
            Quaternion rotation = quaternion.LookRotation(lookAtTarget, Vector3.up);
            transform.LookAt(lookAtTarget);
            //cameraTarget.transform.rotation *= Quaternion.Inverse(rotation);
        }

        private void UpdateAnimator()
        {
            if (movementInput.magnitude > 0.01f)
            {
                _animator.SetBool("IsMoving", true);
                _animator.SetFloat("MovementX", 0.0f);
                _animator.SetFloat("MovementY", 1.0f);
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
                movementInput = new Vector3(input.x, 0, input.y);
            }
        }
        
        
        public void OnLookInput(InputAction.CallbackContext context)
        {
            mousePosition = context.ReadValue<Vector2>();
        }
    }
}