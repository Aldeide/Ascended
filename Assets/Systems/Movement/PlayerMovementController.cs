using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Systems.Movement
{
    public class PlayerMovementController : MonoBehaviour
    {
        private Vector2 movementVector = new Vector2(0, 0);
        public void Start()
        {
            
        }

        public void Update()
        {
            this.transform.position += new Vector3(movementVector.x, 0, movementVector.y) * Time.deltaTime; 
        }

        public void OnMoveAction(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Canceled)
            {
                Debug.Log(context.ReadValue<Vector2>());
                movementVector = context.ReadValue<Vector2>();
            }
        }
    }
}