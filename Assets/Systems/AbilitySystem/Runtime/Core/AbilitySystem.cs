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
    public class AbilitySystemManager : IAbilitySystem, IDisposable
    {
        public INetworkRole NetworkRole { get; set; }
        public EffectManager EffectManager { get; set; }
        public AbilityManager AbilityManager { get; set; }
        public GameplayTagManager TagManager { get; set; }
        public AttributeSetManager AttributeSetManager { get; set; }
        public CueManager CueManager { get; set; }
        public IReplicationManager ReplicationManager { get; set; }
        public IDataManager DataManager { get; set; }
        public EventManager EventManager { get; set; }

        public Action<string, CueData, bool> OnPlayCueRequested;

        public AbilitySystemManager(IDataManager dataManager = null)
        {
            DataManager = dataManager;
            EventManager = new EventManager();
            AttributeSetManager = new AttributeSetManager(this);
            EffectManager = new EffectManager(this);
            AbilityManager = new AbilityManager(this);
            TagManager = new GameplayTagManager(this);
            CueManager = new CueManager(this);
            ReplicationManager = new ReplicationManager(this);
            ReplicationManager.DataManager = dataManager;
            Debug.Log($"[AbilitySystemManager] Created Instance {GetHashCode()}");
        }
        
        public void Tick()
        {
            EffectManager.Tick();
            AbilityManager.Tick();
            AttributeSetManager.UpdateAttributesJobified();
        }

        public void Dispose()
        {
            AttributeSetManager?.Dispose();
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
            PlayCue(cue.CueTag.Name, data, isPredicted);
        }

        public void PlayCue(CueDefinition cue, CueData data, bool isPredicted = false)
        {
            PlayCue(cue.CueTag.Name, data, isPredicted);
        }

        public void PlayCue(string cueTag, CueData data, bool isPredicted = false)
        {
            if (isPredicted && IsLocalClient())
            {
                CueManager.MarkCueAsPredicted(cueTag, data.PredictionKey);
            }
            OnPlayCueRequested?.Invoke(cueTag, data, isPredicted);
        }

        public void PlayCue(GameplayTags.Runtime.Tag cueTag, CueData data, bool isPredicted)
        {
            PlayCue(cueTag.Name, data, isPredicted);
        }

        public void AddCue(CueDefinition cue, CueData data)
        {
            // Placeholder
        }

        public Effect MakeOutgoingEffect(EffectDefinition definition, int level = 1, EffectContext context = null)
        {
            if (context == null)
            {
                context = MakeEffectContext();
            }
            return definition.ToEffect(this, this, context);
        }

        public EffectContext MakeEffectContext()
        {
            return new EffectContext(this, this);
        }

        public EffectApplicationResult ApplyEffectToSelf(Effect effect)
        {
            effect.Initialise(effect.Source, this, effect.Context, effect.Level);
            effect.Activate();
            return EffectManager.AddEffect(effect);
        }

        public void Reset()
        {
            AttributeSetManager.Reset();
        }

        public GameObject GetGameObjectFromNetworkId(ulong networkId)
        {
            return NetworkRole?.GetGameObjectFromNetworkId(networkId);
        }
    }
}