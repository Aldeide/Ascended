using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using UnityEngine;

namespace AbilitySystem.Runtime.Core
{
    public class AbilitySystemManager : IAbilitySystem
    {
        public GameplayTagManager TagManager { get; set; }
        public EffectManager EffectManager { get; set; }
        public AbilityManager AbilityManager { get; set; }
        public AttributeSetManager AttributeSetManager { get; set; }

        public AbilitySystemManager()
        {
            AttributeSetManager = new AttributeSetManager(this);
            TagManager = new GameplayTagManager(this);
            EffectManager = new EffectManager(this);
            AbilityManager = new AbilityManager(this);
            AttributeSetManager = new AttributeSetManager(this);
        }

        public void Tick()
        {
            throw new System.NotImplementedException();
        }

        public float GetTime()
        {
            return Time.time;
        }

        public bool IsLocalClient()
        {
            return true;
        }


    }
}