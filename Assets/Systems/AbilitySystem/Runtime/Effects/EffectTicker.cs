using UnityEngine;

namespace AbilitySystem.Runtime.Effects
{
    public class EffectTicker
    {
        private float _remainingPeriod;
        private readonly Effect _effect;

        public EffectTicker(Effect effect)
        {
            _effect = effect;
            _remainingPeriod = Period;
        }

        public void Tick()
        {
            UpdatePeriod();

            if (_effect.Definition.IsFixedDuration() && _effect.RemainingDuration() <= 0)
            {
                _effect.RemoveSelf();
            }
        }

        private void UpdatePeriod()
        {
            if (Period <= 0) return;
            var deltaTime = Time.deltaTime;
            _remainingPeriod -= deltaTime;
            if (_remainingPeriod <= 0)
            {
                ResetPeriod();
                _effect.PeriodicEffect?.Execute();
            }
        }
        
        public void ResetPeriod()
        {
            _remainingPeriod = Period;
        }

        private float Period => _effect.Definition.GetPeriod();
    }
}