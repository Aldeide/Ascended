using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using Systems.Camera;
using UnityEngine;

namespace AbilitySystemExtension.Runtime.Abilities
{
    public class AimCameraAbility : Ability
    {
        private CameraController _cameraController;
        
        public AimCameraAbility(AbilityDefinition ability, IAbilitySystem owner) : base(ability, owner)
        {
            _cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        }

        public override void ActivateAbility(params object[] args)
        {
            if (!_cameraController) return;
            _cameraController.aimVirtualCamera.SetActive(true);
            
        }

        public override void CancelAbility()
        {
            _cameraController.aimVirtualCamera.SetActive(false);
        }

        public override void EndAbility()
        {
            _cameraController.aimVirtualCamera.SetActive(false);
        }
    }
}