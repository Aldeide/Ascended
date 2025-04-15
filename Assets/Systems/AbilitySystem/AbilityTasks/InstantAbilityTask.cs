using System;
using System.Linq;
using Systems.AbilitySystem.Abilities;
using Systems.AbilitySystem.Authoring.AbilityTasks;
using Systems.AbilitySystem.Data;
using UnityEngine;

namespace Systems.AbilitySystem.AbilityTasks
{
    public abstract class InstantAbilityTask : AbstractAbilityTask
    {
        public abstract void Execute();
    }

    public class InstantTaskData : AbstractAbilityTaskData
    {
        public InstantTaskData()
        {
            TaskData = new JsonData()
            {
                type = typeof(DefaultInstantAbilityTask).FullName,
            };    
        }
        
        public InstantAbilityTask CreateTask(AbilitySpec abilitySpec)
        {
            var task = base.Create(abilitySpec);
            var instantAbilityTask = task as InstantAbilityTask;
            return instantAbilityTask;
        }
        
        public override AbstractAbilityTask Load()
        {
            InstantAbilityTask task = null;
            var jsonData = TaskData.data;
            var dataType = string.IsNullOrEmpty(TaskData.type) ? typeof(DefaultInstantAbilityTask).FullName : TaskData.type;

            var type = InheritedTypes.FirstOrDefault(type => type.FullName == dataType);
            if (type == null)
            {
                Debug.LogError("AbilityTask type not found: " + dataType);
            }
            else
            {
                if (string.IsNullOrEmpty(jsonData))
                    task = Activator.CreateInstance(type) as InstantAbilityTask;
                else
                    task = JsonUtility.FromJson(jsonData, type) as InstantAbilityTask;
            }

            return task;
        }
    }
}