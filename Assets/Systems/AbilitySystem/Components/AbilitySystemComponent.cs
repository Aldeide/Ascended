using System;
using FishNet.Object;
using Systems.AbilitySystem.Effects;
using Systems.AbilitySystem.Tags;
using Systems.Development;
using UnityEngine;

namespace Systems.AbilitySystem.Components
{
    public class AbilitySystemComponent : NetworkBehaviour
    {
        public AttributeSystem AttributesSystem;
        public TagSystem TagSystem;
        public EffectSystem EffectSystem;
        
        public DevelopmentComponent DevelopmentComponent;
        
        public void Start()
        {
            AttributesSystem = new AttributeSystem(this);
            TagSystem = new TagSystem(this);
            EffectSystem = new EffectSystem(this);
            
            AttributesSystem.Initialise(this);
            EffectSystem.Initialise(this);
            DevelopmentComponent = GetComponent<DevelopmentComponent>();
            DevelopmentComponent.Initialise(this);
        }

        [Server]
        public void Update()
        {
            Tick();
        }

        public void Tick()
        {
            EffectSystem.Tick();
        }

        public void AddEffect(EffectSpec effectSpec)
        {
            EffectSystem.AddEffectSpec(this, effectSpec);
        }

        public bool HasAllTags(GameplayTagSet tags)
        {
            return TagSystem.HasAllTags(tags);
        }
        
        public bool HasAnyTags(GameplayTagSet tags)
        {
            return TagSystem.HasAnyTags(tags.Tags);
        }
        
    }
}