using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Runtime.Core
{
    public class AbilitySystemManager : IAbilitySystem
    {
        public AbilitySystemComponent Component { get; set; }
        public EffectManager EffectManager { get; set; }
        public AbilityManager AbilityManager { get; set; }
        public GameplayTagManager TagManager { get; set; }
        public AttributeSetManager AttributeSetManager { get; set; }
        public CueManager CueManager { get; set; }
        public IReplicationManager ReplicationManager { get; set; }
        public EventManager EventManager { get; set; }
        public AbilitySystemManager(AbilitySystemComponent component)
        {
            Component = component;
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
            return (float)Component.NetworkManager.ServerTime.Time;
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

        public void PlayCue(CueDefinition cue)
        {
            var test = new CueData();
            test.VectorData = new[] {Vector3.one, Vector3.one, Vector3.one};
            Debug.Log("Tag:" + cue.CueTag);
            Component.ObserversPlayCueRpc(cue.CueTag.Name, test);
        }

        public void PlayCue(CueDefinition cue, CueData data)
        {
            Component.ObserversPlayCueWithDataRpc(cue.CueTag.Name, data);
        }

        public void AddCue(CueDefinition cue, CueData data)
        {
            
        }

        public void Reset()
        {
            AttributeSetManager.Reset();
        }
    }
}