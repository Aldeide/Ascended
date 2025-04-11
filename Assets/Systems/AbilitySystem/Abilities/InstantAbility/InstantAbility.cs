using System;
using System.Collections.Generic;
using Systems.Abilities;
using Systems.AbilitySystem.Abilities;
using Systems.AbilitySystem.AbilityTasks;
using Systems.AbilitySystem.Components;
using UnityEngine;

namespace Systems.AbilitySystem.Abilities.InstantAbility
{
    public abstract class InstantAbilityT<T> : AbstractAbility<T> where T : InstantAbilityAssetBase
    {
        protected InstantAbilityT(T abilityAsset) : base(abilityAsset)
        {
        }
    }

    public abstract class InstantAbilitySpecT<T> : AbilitySpec<T> where T : AbstractAbility
    {
        private List<InstantAbilityTask> _onActivateTasks = new();
        private List<InstantAbilityTask> _onEndTasks = new();
        protected InstantAbilitySpecT(T ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            Debug.Log("here: " + ability.Asset);
            InstantAbilityAsset asset = (InstantAbilityAsset)ability.Asset;
            foreach (var taskAsset in asset.OnActivateTasks)
            {
                try
                {
                    var task = Activator.CreateInstance(taskAsset.AbilityTaskType(), args: taskAsset) as InstantAbilityTask;
                    _onActivateTasks.Add(task);
                }
                catch (MissingMethodException e)
                {
                    Debug.LogError("Failed to add task: " + taskAsset.AbilityTaskType().FullName);
                }
            }
            foreach (var taskAsset in asset.OnEndTasks)
            {
                try
                {
                    var task = Activator.CreateInstance(taskAsset.AbilityTaskType(), args: taskAsset) as InstantAbilityTask;
                    _onEndTasks.Add(task);
                }
                catch (MissingMethodException e)
                {
                    Debug.LogError("Failed to add task: " + taskAsset.AbilityTaskType().FullName);
                }
            }
        }
        
        public override void ActivateAbility(params object[] args)
        {
            _onActivateTasks.ForEach(task => task.Execute());
        }

        public override void CancelAbility()
        {
            _onEndTasks.ForEach(task => task.Execute());
        }

        public override void EndAbility()
        {
            _onEndTasks.ForEach(task => task.Execute());
        }

        protected override void AbilityTick()
        {
        }
    }
    
    public sealed class InstantAbility : InstantAbilityT<InstantAbilityAssetBase>
    {
        public InstantAbility(InstantAbilityAssetBase abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new InstantAbilitySpec(this, owner);
        }
    }
    
    public sealed class InstantAbilitySpec : InstantAbilitySpecT<InstantAbility>
    {
        public InstantAbilitySpec(InstantAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
        }
    }
    
}