using System;
using Systems.AbilitySystem.Components;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    public class AbilityController : MonoBehaviour
    {
        private AbilitySystemComponent _asc;
        
        public void Start()
        {
            _asc = GetComponent<AbilitySystemComponent>();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                _asc.TryActivateAbility("Aim");
            }
            
            if (context.phase == InputActionPhase.Canceled)
            {
                _asc.EndAbility("Aim");
            }
        }
    }
    
}