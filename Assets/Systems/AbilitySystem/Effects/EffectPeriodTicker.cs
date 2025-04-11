using UnityEngine;

namespace Systems.AbilitySystem.Effects
{
    public class EffectPeriodTicker
    {
        private float _remainingPeriod;
        private readonly EffectSpec _effectSpec;

        public EffectPeriodTicker(EffectSpec effectSpec)
        {
            _effectSpec = effectSpec;
            _remainingPeriod = Period;
        }

        public void Tick()
        {
            // TODO: trigger tick;
            UpdatePeriod();

            if (_effectSpec.DurationType == EffectDurationType.FixedDuration && _effectSpec.RemainingDuration() <= 0)
            {
                _effectSpec.Remove();
            }
        }

        private void UpdatePeriod()
        {
            if (Period <= 0) return;

            var actualDuration = Time.time - _effectSpec.ActivationTime;
            if (actualDuration < Mathf.Epsilon)
            {
                return;
            }

            var deltaTime = Time.deltaTime;
            var excessDuration = actualDuration - _effectSpec.Duration;
            if (excessDuration >= 0)
            {
                deltaTime -= excessDuration;
                deltaTime += 0.0001f;
            }

            _remainingPeriod -= deltaTime;

            while (_remainingPeriod < 0)
            {
                _remainingPeriod += Period;
                _effectSpec.PeriodicEffect?.TriggerOnExecute();
            }
        }
        
        public void ResetPeriod()
        {
            _remainingPeriod = Period;
        }

        private float Period => _effectSpec.Effect.Period;
    }
}