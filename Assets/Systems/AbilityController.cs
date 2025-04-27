using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Scripts;
using Systems.Targeting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    public class AbilityController : MonoBehaviour
    {
        private AbilitySystemComponent _asc;
        private WeaponController _weaponController;
        private PlayerTargetController _targetController;

        public void Start()
        {
            _asc = GetComponent<AbilitySystemComponent>();
            _weaponController = GetComponent<WeaponController>();
            _targetController = GetComponent<PlayerTargetController>();
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
                _asc.TryActivateAbility("FireAbility", _weaponController.GetMuzzlePosition(),
                    _targetController.GetTargetPosition());
            }

            if (context.phase == InputActionPhase.Canceled)
            {
                _asc.EndAbility("FireAbility");
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                _asc.TryActivateAbility("DashAbility");
            }
        }
    }
}