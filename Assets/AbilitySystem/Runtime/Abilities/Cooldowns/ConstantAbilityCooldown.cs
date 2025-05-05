using System;
using UnityEngine;

namespace AbilitySystem.Runtime.Abilities.Cooldowns
{
    [Serializable]
    public class ConstantAbilityCooldown : AbilityCooldown
    {
        public override float Calculate()
        {
            return BaseDuration;
        }

        public override float RemainingCooldown()
        {
            return _currentCooldown;
        }

        public override void Tick()
        {
            if (_currentCooldown == 0) return;
            _currentCooldown -= Time.deltaTime;
            if (_currentCooldown < 0) _currentCooldown = 0;
        }
    }
}