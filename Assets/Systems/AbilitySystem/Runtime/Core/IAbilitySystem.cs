using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;
using UnityEngine;

namespace AbilitySystem.Runtime.Core
{
    public interface IAbilitySystem
    {
        public INetworkRole NetworkRole { get; set; }
        public GameplayTagManager TagManager { get; set; }
        public EffectManager EffectManager { get; set; }
        public AbilityManager AbilityManager { get; set; }
        public AttributeSetManager AttributeSetManager { get; set; }
        public CueManager CueManager { get; set; }
        public IReplicationManager ReplicationManager { get; set; }
        public IDataManager DataManager { get; set; }
        public EventManager EventManager { get; set; }
        
        public void Tick();

        public float GetTime();
        
        public bool IsLocalClient();

        public bool IsServer();

        public bool IsHost();

        public bool HasAuthority();
        void PlayCue(CueDefinition cue, bool isPredicted = false);
        void PlayCue(CueDefinition cue, CueData data, bool isPredicted = false);
        void PlayCue(string cueTag, CueData data, bool isPredicted = false);
        void PlayCue(GameplayTags.Runtime.Tag cueTag, CueData data, bool isPredicted);

        public Effect MakeOutgoingEffect(EffectDefinition definition, int level = 1, EffectContext context = null);
        public EffectContext MakeEffectContext();
        public EffectApplicationResult ApplyEffectToSelf(Effect effect);
        public void Reset();
        public GameObject GetGameObjectFromNetworkId(ulong networkId);
    }
}