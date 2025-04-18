using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;
using UnityEngine;

namespace AbilitySystem.Runtime.Core
{
    public class AbilitySystemManager : IAbilitySystem
    {
        public AbilitySystemComponent Component { get; set; }
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

        public void Initialise(AbilitySystemComponent component)
        {
            Component = component;
        }

        public void Tick()
        {
            EffectManager.Tick();
            
        }

        public float GetTime()
        {
            return Time.time;
        }

        public bool IsLocalClient()
        {
            return Component.IsLocalPlayer;
        }

        public bool IsServer()
        {
            return Component.IsServer;
        }

        public bool IsHost()
        {
            return Component.IsHost;
        }

        public bool HasAuthority()
        {
            return Component.HasAuthority;
        }
    }
}