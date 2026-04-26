using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystemExtension.Scripts;
using Systems.Camera;
using Systems.Controllers;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class AimCameraAbility : Ability
    {
        CameraController _cameraController;
        PlayerMovementController _playerController;
        public AimCameraAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
            _cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
            _playerController = ((Component)Owner.NetworkRole)?.gameObject.GetComponent<PlayerMovementController>();
        }

        protected override void ActivateAbility(AbilityData data)
        {
            _cameraController.aimVirtualCamera.SetActive(true);
        }

        protected override void CancelAbility()
        {
            EndAbility();
        }

        public override void EndAbility()
        {
            _cameraController.aimVirtualCamera.SetActive(false);
        }
    }
}
