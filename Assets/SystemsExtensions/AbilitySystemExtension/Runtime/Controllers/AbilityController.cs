using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Scripts;
using AbilitySystemExtension.Scripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Item.Scripts.UI;

namespace Systems.Controllers
{
    public class AbilityController : NetworkBehaviour
    {
        private AbilitySystemComponent _asc;
        private WeaponController _weaponController;
        private PlayerTargetController _targetController;

        private PlayerMovementController _movementController;
        private float _combatStanceTimer = 0f;

        private InventoryUIController _inventoryUI;

        private bool IsInventoryOpen()
        {
            if (_inventoryUI == null)
            {
                _inventoryUI = GetComponentInChildren<InventoryUIController>();
            }
            return _inventoryUI != null && _inventoryUI.IsMenuOpen;
        }

        public void CancelActiveActions()
        {
            if (!IsLocalPlayer) return;
            
            if (_asc != null)
            {
                _asc.EndAbility("AimCameraAbility");
                _asc.EndAbility("FireAbility");
            }
            
            if (_movementController != null)
            {
                _movementController.SetManualAiming(false);
                _movementController.SetCombatStance(false);
            }
            
            _combatStanceTimer = 0f;
        }

        public void Start()
        {
            _asc = GetComponent<AbilitySystemComponent>();
            _weaponController = GetComponent<WeaponController>();
            _targetController = GetComponent<PlayerTargetController>();
            _movementController = GetComponent<PlayerMovementController>();
        }

        public void Update()
        {
            if (!IsLocalPlayer) return;
            if (!(_combatStanceTimer > 0)) return;
            _combatStanceTimer -= Time.deltaTime;
            if (_combatStanceTimer <= 0)
            {
                _movementController.SetCombatStance(false);
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (!IsLocalPlayer) return;
            if (IsInventoryOpen()) return;
            if (context.phase == InputActionPhase.Started)
            {
                _movementController.SetManualAiming(true);
                _asc.TryActivateAbility("AimCameraAbility");
            }

            if (context.phase != InputActionPhase.Canceled) return;
            _movementController.SetManualAiming(false);
            _asc.EndAbility("AimCameraAbility");
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (!IsLocalPlayer) return;
            if (IsInventoryOpen()) return;
            if (context.phase == InputActionPhase.Started)
            {
                var data = new AbilityData
                {
                    MuzzlePosition = _weaponController.GetMuzzlePosition(),
                    TargetPosition = _targetController.GetTargetPosition()
                };
                
                // Enter combat stance
                _movementController.SetCombatStance(true);
                _combatStanceTimer = 2f;

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
            if (IsInventoryOpen()) return;
            if (context.phase != InputActionPhase.Performed) return;
            _asc.TryActivateAbility("DashAbility");
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!IsLocalPlayer) return;
            if (IsInventoryOpen()) return;
            if (context.phase != InputActionPhase.Performed) return;
            _asc.TryActivateAbility("JumpAbility");
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (!IsLocalPlayer) return;
            if (IsInventoryOpen()) return;
            if (context.phase != InputActionPhase.Performed) return;
            _asc.TryActivateAbility("ReloadWeaponAbility");
        }
    }
}