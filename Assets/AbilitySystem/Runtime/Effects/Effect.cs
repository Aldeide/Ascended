using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using AbilitySystem.Runtime.Attributes;
using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Effects
{
    public class Effect
    {
        public EffectDefinition Definition { get; private set; }
        public float Duration { get; private set; }
        public bool IsActive { get; private set; }
        public float ActivationTime { get; private set; }
        public IAbilitySystem Owner { get; private set; }
        public IAbilitySystem Source { get; private set; }

        public Dictionary<string, AttributeValue> OwnerAttributeSnapshot { get; private set; }
        
        public Dictionary<string, AttributeValue> SourceAttributeSnapshot { get; private set; }
        
        private EffectTicker _effectTicker = null;
        public Effect PeriodicEffect { get; private set; }
        
        public Effect(EffectDefinition definition)
        {
            Definition = definition;
            Duration = Definition.Asset.durationSeconds;
            if (!Definition.IsInstant()) _effectTicker = new EffectTicker(this);
        }

        public void Initialise(IAbilitySystem source, IAbilitySystem target)
        {
            Owner = target;
            Source = source;
            if (Definition.Asset.periodicEffect && (Definition.IsInfinite() || Definition.IsFixedDuration()))
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
        }

        public void Tick()
        {
            if (Definition.IsInstant()) return;
            if (!IsActive) return;
            _effectTicker.Tick();
        }

        public void RemoveSelf()
        {
            Owner.EffectManager.RemoveEffect(this);
        }

        public void Execute()
        {
            Debug.Log("Execute");
            if (!Definition.IsInstant()) return;
            Owner.AttributeSetManager.ApplyInstantEffectModifiers(this);
        }

        public float RemainingDuration()
        {
            if (Definition.IsInfinite())
                return -1;

            return Mathf.Max(0, Duration - (Owner.GetTime() - ActivationTime));
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

            return $"{Definition.Asset.name} ({typeDuration})";
        }
    }
}