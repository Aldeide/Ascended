using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using UnityEngine;

namespace AbilitySystem.Runtime.Core
{
    public class AbilitySystemManager : IAbilitySystem
    {
        public INetworkRole NetworkRole { get; set; }
        public EffectManager EffectManager { get; set; }
        public AbilityManager AbilityManager { get; set; }
        public GameplayTagManager TagManager { get; set; }
        public AttributeSetManager AttributeSetManager { get; set; }
        public CueManager CueManager { get; set; }
        public IReplicationManager ReplicationManager { get; set; }
        public EventManager EventManager { get; set; }

        public Action<string, CueData, bool> OnPlayCueRequested;

        public AbilitySystemManager()
        {
            EventManager = new EventManager();
            AttributeSetManager = new AttributeSetManager(this);
            EffectManager = new EffectManager(this);
            AbilityManager = new AbilityManager(this);
            TagManager = new GameplayTagManager(this);
            CueManager = new CueManager(this);
            ReplicationManager = new ReplicationManager(this);
        }
        
        public void Tick()
        {
            EffectManager.Tick();
            AbilityManager.Tick();
        }

        public float GetTime()
        {
            if (NetworkRole == null) return Time.time;
            return (float)NetworkRole.Time;
        }

        public bool IsLocalClient()
        {
            if (NetworkRole == null) return true;
            return NetworkRole.IsLocalPlayer;
        }

        public bool IsServer()
        {
            if (NetworkRole == null) return true;
            return NetworkRole.IsServer;
        }

        public bool IsHost()
        {
            if (NetworkRole == null) return true;
            return NetworkRole.IsHost;
        }

        public bool HasAuthority()
        {
            if (NetworkRole == null) return true;
            return NetworkRole.HasAuthority;
        }

        public void PlayCue(CueDefinition cue, bool isPredicted = false)
        {
            var data = new CueData();
            data.VectorData = new[] {Vector3.one, Vector3.one, Vector3.one};
            Debug.Log("Tag:" + cue.CueTag);
            OnPlayCueRequested?.Invoke(cue.CueTag.Name, data, isPredicted);
        }

        public void PlayCue(CueDefinition cue, CueData data, bool isPredicted = false)
        {
            OnPlayCueRequested?.Invoke(cue.CueTag.Name, data, isPredicted);
        }

        public void PlayCue(string cueTag, CueData data, bool isPredicted = false)
        {
            OnPlayCueRequested?.Invoke(cueTag, data, isPredicted);
        }

        public void AddCue(CueDefinition cue, CueData data)
        {
            // Placeholder
        }

        public void Reset()
        {
            AttributeSetManager.Reset();
        }
    }
}