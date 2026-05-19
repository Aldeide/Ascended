using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystem.Runtime.Effects
{
    public class Effect
    {
        public EffectDefinition Definition { get; private set; }
        public float Duration { get; set; }
        public bool IsActive { get; set; }
        public float ActivationTime { get; set; }
        public int NumStacks { get; set; }
        public int Level { get; set; }
        public IAbilitySystem Owner { get; private set; }
        public IAbilitySystem Source { get; private set; }

        public Dictionary<string, float> OwnerCapturedAttributes { get; private set; } = new();
        public Dictionary<string, float> SourceCapturedAttributes { get; private set; } = new();
        public Dictionary<Tag, float> SetByCallerTagMagnitudes { get; private set; } = new();
        public EffectContext Context { get; private set; }
        public float Period { get; set; }
        public Effect PeriodicEffect { get; private set; }
        public PredictionKey PredictionKey { get; set; }
        public bool IsReplicated { get; set; }
        public Guid Guid;
        
        private readonly EffectTicker _effectTicker;
        public Effect(EffectDefinition definition)
        {
            Definition = definition;
            Duration = Definition.DurationSeconds;
            NumStacks = 1;
            if (!Definition.IsInstant()) _effectTicker = new EffectTicker(this);
        }

        public void Initialise(IAbilitySystem source, IAbilitySystem target, int level = 1)
        {
            Initialise(source, target, new EffectContext(source, source), level);
        }

        public void Initialise(IAbilitySystem source, IAbilitySystem target, EffectContext context, int level = 1)
        {
            Owner = target;
            Source = source;
            Context = context;
            Level = level;
            Duration = Definition.DurationSeconds;
            Period = Definition.Period;
            NumStacks = 1;
            if (Definition.PeriodicEffect && (Definition.IsInfinite() || Definition.IsFixedDuration()))
            {
                PeriodicEffect = Definition.GetPeriodicEffectDefinition().ToEffect(source, target, context);
            }

            CaptureAttributes();
        }

        private void CaptureAttributes()
        {
            if (Definition.Modifiers != null)
            {
                foreach (var modifier in Definition.Modifiers)
                {
                    modifier.CaptureAttributes(this);
                }
            }
        }


        public void Activate()
        {
            if (IsActive) return;
            ActivationTime = Owner.GetTime();
            IsActive = true;
            if (Definition.OngoingRequiredTags != null && Definition.OngoingRequiredTags.Length > 0)
            {
                Owner.TagManager.OnTagsChanged += EvaluateOngoingTags;
                EvaluateOngoingTags();
            }
            PlayApplicationCues();
        }

        public void EvaluateOngoingTags()
        {
            if (Definition.OngoingRequiredTags == null || Definition.OngoingRequiredTags.Length == 0) return;
            var hasTags = Owner.TagManager.HasAllTags(Definition.OngoingRequiredTags);
            if (IsActive == hasTags) return;
            
            IsActive = hasTags;
            if (!IsActive) 
            {
               Owner.EffectManager.OnEffectSuspended?.Invoke(this); 
            }
            else 
            {
               Owner.EffectManager.OnEffectResumed?.Invoke(this); 
            }
        }

        public void PlayApplicationCues()
        {
            if (Definition.Cues == null) return;
            foreach (var cue in Definition.Cues)
            {
                Owner.PlayCue(cue, IsPredicted());
            }
        }

        public void Tick()
        {
            if (Definition.IsInstant()) return;
            if (!IsActive) return;
            _effectTicker.Tick();
        }

        public void SetLevel(int level)
        {
            Level = level;
        }

        public void RemoveSelf()
        {
            if (Definition.OngoingRequiredTags != null && Definition.OngoingRequiredTags.Length > 0)
            {
                Owner.TagManager.OnTagsChanged -= EvaluateOngoingTags;
            }

            if (Definition.EffectStack.EffectStackType != EffectStackType.None && NumStacks > 1)
            {
                if (Definition.EffectStack.EffectStackExpirationPolicy ==
                    EffectStackExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                {
                    var oldStacks = NumStacks;
                    NumStacks -= 1;
                    RefreshDuration();
                    Owner.EffectManager.OnEffectStacksChanged?.Invoke(this, oldStacks, NumStacks);
                    return;
                }
            }
            Owner.EffectManager.RemoveEffect(this);
        }

        public void Execute()
        {
            if (!Definition.IsInstant()) return;
            Owner.AttributeSetManager.ApplyInstantEffectModifiers(this);
        }

        public float RemainingDuration()
        {
            if (Definition.IsInfinite())
                return -1;

            return Mathf.Max(0, Duration - (Owner.GetTime() - ActivationTime));
        }

        public void AddStack()
        {
            var maxStacks = Definition.EffectStack.MaxStacks;
            var oldStacks = NumStacks;
            if (NumStacks < maxStacks)
            {
                NumStacks++;
            }

            if (Definition.EffectStack.EffectStackDurationPolicy == EffectStackDurationPolicy.RefreshOnNewApplication)
            {
                RefreshDuration();
            }
            
            _effectTicker?.ResetPeriod();
            
            if (oldStacks != NumStacks)
            {
                Owner.EffectManager.OnEffectStacksChanged?.Invoke(this, oldStacks, NumStacks);
            }
        }

        public void RefreshDuration()
        {
            ActivationTime = Owner.GetTime();
        }

        public void SetSetByCallerMagnitude(Tag tag, float magnitude)
        {
            SetByCallerTagMagnitudes[tag] = magnitude;
        }

        public float GetSetByCallerMagnitude(Tag tag, float defaultValue = 0f)
        {
            return SetByCallerTagMagnitudes.GetValueOrDefault(tag, defaultValue);
        }
        
        public bool IsPredictable() 
        {
            return Definition.DurationType != EffectDurationType.Instant;
        }

        public bool IsPredicted()
        {
            return PredictionKey.IsValidKey();
        }

        public string DebugString()
        {
            var typeDuration = "";
            if (Definition.IsInfinite())
            {
                typeDuration = "Infinite";
            }
            else
            {
                typeDuration = RemainingDuration().ToString(CultureInfo.InvariantCulture);
            }

            return $"{Definition.name} ({typeDuration}) Stacks: {NumStacks}";
        }
    }
}