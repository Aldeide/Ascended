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
        
        public EffectSpec(Effect effect)
        {
            this.Effect = effect;
            Duration = effect.Duration;
            Modifiers = effect.Modifiers;
            DurationType = effect.EffectDurationType;
        }

        public void Initialise(AbilitySystemComponent creator, AbilitySystemComponent owner, float level)
        {
            Source = creator;
            Owner = owner;
            Level = level;
        }

        public void Tick()
        {
            if (DurationType != EffectDurationType.FixedDuration) return;
            if (ActivationTime + Duration <= Time.time)
            {
                Debug.Log("Removing");
                Remove();
            }
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
            
        }
        
        public float RemainingDuration()
        {
            if (DurationType == EffectDurationType.Infinite)
                return -1;

            return Mathf.Max(0, Duration - (Time.time - ActivationTime));
        }
    }
}