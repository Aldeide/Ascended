using System;
using Authoring.AbilityTasks;
using Systems.AbilitySystem.AbilityTasks;
using Systems.AbilitySystem.Authoring.AbilityTasks;
using UnityEngine;

namespace Authoring.Classes.AbilityTasksAssets
{
    [CreateAssetMenu(fileName = "SetCameraStateTask", menuName = "AbilitySystem/AbilityTasks/SetCameraStateTask")]
    public class SetCameraStateTaskAsset : AbilityTaskAsset
    {
        public string CameraName;
        public bool SetActive;
        
        public override Type AbilityTaskType()
        {
            return typeof(SetCameraStateTask);
        }
    }
}