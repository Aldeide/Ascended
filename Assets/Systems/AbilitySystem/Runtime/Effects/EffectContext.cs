using AbilitySystem.Runtime.Core;
using UnityEngine;

namespace AbilitySystem.Runtime.Effects
{
    public class EffectContext
    {
        public IAbilitySystem Instigator { get; private set; }
        public IAbilitySystem EffectCauser { get; private set; }
        public Vector3 Origin { get; set; }
        public RaycastHit HitResult { get; set; }
        public bool HasHitResult { get; set; }

        public EffectContext(IAbilitySystem instigator, IAbilitySystem effectCauser)
        {
            Instigator = instigator;
            EffectCauser = effectCauser;
        }

        public virtual EffectContext Duplicate()
        {
            var context = new EffectContext(Instigator, EffectCauser)
            {
                Origin = Origin,
                HitResult = HitResult,
                HasHitResult = HasHitResult
            };
            return context;
        }
    }
}
