using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Systems.Movement
{
    public class PlayerMovementController : MonoBehaviour
    {
        private Vector3 movementInput = new Vector3(0, 0, 0);
        private Vector2 mousePosition = new Vector2(0, 0);
        
        private Animator _animator;
        private Rigidbody _rigidbody;
        
        public void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Update()
        {
            Vector3 newPos = this.transform.position += Quaternion.AngleAxis(45, Vector3.up) * movementInput * (Time.deltaTime * 3.0f) * 4.0f;
            _rigidbody.MovePosition(newPos);
            //Rotate();
            UpdateAnimator();
        }
        
        private void Rotate()
        {
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 target = hit.point;
                target.y = transform.position.y;
                transform.LookAt(target);
                // Debug.DrawLine(this.transform.position, hit.point);
            }
        }

        private void UpdateAnimator()
        {
            if (movementInput.magnitude > 0.01f)
            {
                _animator.SetBool("IsMoving", true);
                _animator.SetFloat("MovementX", movementInput.x);
                _animator.SetFloat("MovementY", movementInput.y);
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