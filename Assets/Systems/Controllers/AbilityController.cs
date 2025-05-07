using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Scripts;
using Unity.Netcode;
using UnityEngine.InputSystem;

namespace Systems.Controllers
{
    public class AbilityController : NetworkBehaviour
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
            if (!IsLocalPlayer) return;
            if (context.phase == InputActionPhase.Started)
            {
                _asc.TryActivateAbility("TestAbilityGraph");
            }

            if (context.phase == InputActionPhase.Canceled)
            {
                _asc.EndAbility("TestAbilityGraph");
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (!IsLocalPlayer) return;
            if (context.phase == InputActionPhase.Started)
            {
                var data = new AbilityData
                {
                    MuzzlePosition = _weaponController.GetMuzzlePosition(),
                    TargetPosition = _targetController.GetTargetPosition()
                };
                _asc.TryActivateAbility("FireAbility", data);
            }

            if (context.phase == InputActionPhase.Canceled)
            {
                _asc.EndAbility("FireAbility");
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (!IsLocalPlayer) return;
            if (context.phase == InputActionPhase.Performed)
            {
                _asc.TryActivateAbility("DashAbility");
            }
        }
    }
}