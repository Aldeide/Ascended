using AbilitySystem.Scripts;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using GameplayTags.Runtime;
using UnityEngine;

namespace AISystem.Runtime.Sensors
{
    public class TagSensor : LocalWorldSensorBase
    {
        public string TagName { get; set; }
        public bool CheckTarget { get; set; }

        public override void Created() {}

        public override void Update() {}

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            if (string.IsNullOrEmpty(TagName))
                return false;

            AbilitySystemComponent asc = null;

            if (CheckTarget)
            {
                var action = agent.ActionState.Action;
                var data = agent.ActionState.Data;
                if (action != null && data != null && data.Target != null)
                {
                    if (data.Target is TransformTarget transformTarget && transformTarget.Transform != null)
                    {
                        asc = transformTarget.Transform.GetComponent<AbilitySystemComponent>();
                    }
                }
            }
            else
            {
                asc = agent.Transform.GetComponent<AbilitySystemComponent>();
            }

            if (asc == null || !asc.IsInitialized)
                return false;

            var tagToCheck = new Tag(TagName);
            return asc.AbilitySystem.TagManager.HasTag(tagToCheck);
        }
    }
}
