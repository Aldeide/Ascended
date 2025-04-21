using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
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
                _asc.TryActivateAbility("AimCameraAbility");
            }
            
            if (context.phase == InputActionPhase.Canceled)
            {
                _asc.EndAbility("AimCameraAbility");
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                _asc.TryActivateAbility("FireAbility");
            }
            
            if (context.phase == InputActionPhase.Canceled)
            {
                _asc.EndAbility("FireAbility");
            }
        }
    }
    
}