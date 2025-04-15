using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Systems.AbilitySystem.Abilities;
using Systems.AbilitySystem.Data;
using Systems.AbilitySystem.Util;
using UnityEngine;

namespace Systems.AbilitySystem.AbilityTasks
{
    public abstract class AbstractAbilityTask
    {
        protected AbilitySpec AbilitySpec;

        public virtual void Initialise(AbilitySpec abilitySpec)
        {
            AbilitySpec = abilitySpec;
        }
    }

    public abstract class AbstractAbilityTaskData
    {
        public JsonData TaskData;

        public virtual AbstractAbilityTask Create(AbilitySpec abilitySpec)
        {
            var task = Load();
            task.Initialise(abilitySpec);
            return task;
        }

        public void Save(AbstractAbilityTask task)
        {
            var jsonData = JsonUtility.ToJson(task);
            var dataType = task.GetType().FullName;
            TaskData = new JsonData { data = jsonData, type = dataType };
        }
        
        public abstract AbstractAbilityTask Load();

        private static Type[] _inheritedTypes;

        public static Type[] InheritedTypes => _inheritedTypes ?? TypeUtil.GetAllInheritedTypesOf(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<string> InheritedTypesList
        {
            get
            {
                return InheritedTypes.Select(type => type.FullName).ToList();
            }
        }
    }
}