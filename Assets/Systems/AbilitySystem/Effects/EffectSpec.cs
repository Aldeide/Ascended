using System;
using Systems.AbilitySystem.Components;
using Systems.AbilitySystem.Effects.Modifiers;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Systems.AbilitySystem.Effects
{
    public class EffectSpec
    {
        public Effect Effect { get; }

        public float Level { get; private set; }
        public AbilitySystemComponent Source { get; private set; }
        public AbilitySystemComponent Owner { get; private set; }
        public bool IsApplied { get; private set; }
        public bool IsActive { get; private set; }
        public float Duration { get; private set; }
        public float ActivationTime { get; private set; }
        
        public EffectDurationType DurationType { get; private set; }
        public EffectModifier[] Modifiers { get; private set; }
        public EffectSpec PeriodicEffect { get; private set; }
        public EffectPeriodTicker EffectPeriodTicker { get; }
        public EffectStack EffectStack { get; private set; }
        public int StackCount { get; private set; } = 1;
        
        public event Action<int,int> onStackCountChanged;
        
        public EffectSpec(Effect effect)
        {
            this.Effect = effect;
            Duration = effect.Duration;
            Modifiers = effect.Modifiers;
            DurationType = effect.EffectDurationType;
            EffectStack = effect.EffectStack;
            
            if (DurationType != EffectDurationType.Instant)
            {
                EffectPeriodTicker = new EffectPeriodTicker(this);
            }
        }

        public void Initialise(AbilitySystemComponent creator, AbilitySystemComponent owner, float level)
        {
            Source = creator;
            Owner = owner;
            Level = level;
            if (Effect.EffectDurationType != EffectDurationType.Instant)
            {
                PeriodicEffect = Effect.PeriodicEffect?.ToEffectSpec(creator, owner);
            }
        }

        public void Tick()
        {
            EffectPeriodTicker?.Tick();
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
            ActivationTime = Time.time;
        }

        public void Remove()
        {
            Owner.EffectSystem.RemoveEffect(this);
        }

        public void TriggerOnExecute()
        {
            // TODO: remove all effects with 'RemoveEffectsWithTag'
            Owner.ApplyModifierFromInstantGameplayEffect(this);
        }
        
        public float RemainingDuration()
        {
            if (DurationType == EffectDurationType.Infinite)
                return -1;

            return Mathf.Max(0, Duration - (Time.time - ActivationTime));
        }

        #region Stacks

        public bool AddStack()
        {
            var previousStacks = StackCount;
            SetStacks(previousStacks + 1);
            OnStackCountChange(previousStacks, StackCount);
            return previousStacks != StackCount;
        }
        
        
        public void SetStacks(int stackCount)
        {
            if (stackCount <= EffectStack.MaxStacks)
            {
                StackCount = Mathf.Max(1, stackCount);
                if (EffectStack.EffectStackDurationPolicy == EffectStackDurationPolicy.RefreshOnNewApplication)
                {
                    RefreshDuration();
                }

                if (EffectStack.EffectStackPeriodPolicy == EffectStackPeriodPolicy.RefreshOnNewApplication)
                {
                    EffectPeriodTicker.ResetPeriod();
                }

                return;
            }
            
            // TODO: stack overflow behaviour.
        }
        
        private void OnStackCountChange(int oldStackCount, int newStackCount)
        {
            
            onStackCountChanged?.Invoke(oldStackCount, newStackCount);
        }
        
        public void RegisterOnStackCountChanged(Action<int, int> callback)
        {
            onStackCountChanged += callback;
        }

        public void UnregisterOnStackCountChanged(Action<int, int> callback)
        {
            onStackCountChanged -= callback;
        }

        #endregion
        
        public void RefreshDuration()
        {
            ActivationTime = Time.time;
        }
    }
}