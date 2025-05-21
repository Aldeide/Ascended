using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
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
        
        public IAbilitySystem Owner { get; private set; }
        public IAbilitySystem Source { get; private set; }

        public Dictionary<string, AttributeValue> OwnerAttributeSnapshot { get; private set; }
        public Dictionary<string, AttributeValue> SourceAttributeSnapshot { get; private set; }
        public Effect PeriodicEffect { get; private set; }
        public PredictionKey PredictionKey { get; set; }
        public Guid Guid;
        
        private readonly EffectTicker _effectTicker;
        public Effect(EffectDefinition definition)
        {
            Definition = definition;
            Duration = Definition.durationSeconds;
            if (!Definition.IsInstant()) _effectTicker = new EffectTicker(this);
        }

        public void Initialise(IAbilitySystem source, IAbilitySystem target)
        {
            Owner = target;
            Source = source;
            NumStacks = 1;
            if (Definition.periodicEffect && (Definition.IsInfinite() || Definition.IsFixedDuration()))
            {
                PeriodicEffect = Definition.GetPeriodicEffectDefinition().ToEffect(source, target);
            }
        }

        public void Activate()
        {
            if (IsActive) return;
            ActivationTime = Owner.GetTime();
            OwnerAttributeSnapshot = Owner.AttributeSetManager.Snapshot();
            SourceAttributeSnapshot = Source.AttributeSetManager.Snapshot();
            IsActive = true;
            PlayApplicationCues();
        }

        public void PlayApplicationCues()
        {
            if (Definition.cues == null) return;
            foreach (var cue in Definition.cues)
            {
                Owner.PlayCue(cue);
            }
        }

        public void Tick()
        {
            if (Definition.IsInstant()) return;
            if (!IsActive) return;
            _effectTicker.Tick();
        }

        public void RemoveSelf()
        {
            if (Definition.EffectStack.EffectStackType != EffectStackType.None && NumStacks > 1)
            {
                if (Definition.EffectStack.EffectStackExpirationPolicy ==
                    EffectStackExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                {
                    NumStacks -= 1;
                    RefreshDuration();
                    // TODO: make a 'effect stacks changed' event.
                    Owner.EffectManager.OnEffectRemoved?.Invoke(this);
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
            if (NumStacks < maxStacks)
            {
                NumStacks++;
            }

            if (Definition.EffectStack.EffectStackDurationPolicy == EffectStackDurationPolicy.RefreshOnNewApplication)
            {
                RefreshDuration();
            }
            // TODO: refresh period for ticking effects.
        }

        public void RefreshDuration()
        {
            ActivationTime = Owner.GetTime();
        }
        
        public bool IsPredictable() 
        {
            return Definition.durationType != EffectDurationType.Instant;
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