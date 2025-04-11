using Authoring.Classes.AbilityTasksAssets;
using Systems.AbilitySystem.AbilityTasks;
using Systems.AbilitySystem.Authoring.AbilityTasks;
using Systems.Camera;
using UnityEngine;

namespace Authoring.AbilityTasks
{
    public class SetCameraStateTask : InstantAbilityTask
    {
        private string _cameraName;
        private bool _setActive;
        private CameraController _cameraController;
        
        public SetCameraStateTask(SetCameraStateTaskAsset asset)
        {
            Debug.Log("SetCamera");
            _cameraName = asset.CameraName;
            _setActive = asset.SetActive;
            _cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        }
        
        public override void Execute()
        {
            if (!_cameraController) return;
            if (_cameraName == "AimCamera")
            {
                _cameraController.aimVirtualCamera.SetActive(_setActive);
            }
            if (_cameraName == "DefaultCamera")
            {
                _cameraController.defaultVirtualCamera.SetActive(_setActive);
            }
        }
    }


}