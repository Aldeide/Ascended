using System;
using UnityEngine;

namespace Systems.AbilitySystem.Authoring.AbilityTasks
{
    public abstract class AbilityTaskAsset : ScriptableObject
    {
        public string name;
        public abstract Type AbilityTaskType();
    }
}