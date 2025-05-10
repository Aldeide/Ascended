using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;

namespace AbilitySystem.Runtime.Core
{
    public interface IAbilitySystem
    {
        public AbilitySystemComponent Component { get; set; }
        public GameplayTagManager TagManager { get; set; }
        public EffectManager EffectManager { get; set; }
        public AbilityManager AbilityManager { get; set; }
        public AttributeSetManager AttributeSetManager { get; set; }
        public IReplicationManager ReplicationManager { get; set; }
        public void Initialise(AbilitySystemComponent component);
        
        public void Tick();

        public float GetTime();
        
        public bool IsLocalClient();

        public bool IsServer();

        public bool IsHost();

        public bool HasAuthority();
        public void PlayCue(CueDefinition cue);
        public void PlayCue(CueDefinition cue, CueData data);

        public void Reset();
    }
}