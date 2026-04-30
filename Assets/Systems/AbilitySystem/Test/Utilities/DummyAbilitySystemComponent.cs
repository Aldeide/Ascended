using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.AttributeSets;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Events;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Runtime.Tags;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    /// <summary>
    /// A MonoBehaviour that implements IAbilitySystem by delegating to a mock.
    /// Useful for testing components that require an IAbilitySystem on a GameObject.
    /// </summary>
    public class DummyAbilitySystemComponent : MonoBehaviour, IAbilitySystem
    {
        public IAbilitySystem MockSystem;

        public INetworkRole NetworkRole { get => MockSystem?.NetworkRole; set { if (MockSystem != null) MockSystem.NetworkRole = value; } }
        public EffectManager EffectManager { get => MockSystem?.EffectManager; set { if (MockSystem != null) MockSystem.EffectManager = value; } }
        public AbilityManager AbilityManager { get => MockSystem?.AbilityManager; set { if (MockSystem != null) MockSystem.AbilityManager = value; } }
        public GameplayTagManager TagManager { get => MockSystem?.TagManager; set { if (MockSystem != null) MockSystem.TagManager = value; } }
        public AttributeSetManager AttributeSetManager { get => MockSystem?.AttributeSetManager; set { if (MockSystem != null) MockSystem.AttributeSetManager = value; } }
        public CueManager CueManager { get => MockSystem?.CueManager; set { if (MockSystem != null) MockSystem.CueManager = value; } }
        public IReplicationManager ReplicationManager { get => MockSystem?.ReplicationManager; set { if (MockSystem != null) MockSystem.ReplicationManager = value; } }
        public IDataManager DataManager { get => MockSystem?.DataManager; set { if (MockSystem != null) MockSystem.DataManager = value; } }
        public EventManager EventManager { get => MockSystem?.EventManager; set { if (MockSystem != null) MockSystem.EventManager = value; } }

        public void Tick() => MockSystem?.Tick();
        public float GetTime() => MockSystem?.GetTime() ?? 0f;
        public bool IsLocalClient() => MockSystem?.IsLocalClient() ?? false;
        public bool IsServer() => MockSystem?.IsServer() ?? false;
        public bool IsHost() => MockSystem?.IsHost() ?? false;
        public bool HasAuthority() => MockSystem?.HasAuthority() ?? false;
        public void PlayCue(CueDefinition cue, bool isPredicted = false) => MockSystem?.PlayCue(cue, isPredicted);
        public void PlayCue(CueDefinition cue, CueData data, bool isPredicted = false) => MockSystem?.PlayCue(cue, data, isPredicted);
        public void PlayCue(string cueTag, CueData data, bool isPredicted = false) => MockSystem?.PlayCue(cueTag, data, isPredicted);
        public void PlayCue(GameplayTags.Runtime.Tag cueTag, CueData data, bool isPredicted) => MockSystem?.PlayCue(cueTag, data, isPredicted);
        public Effect MakeOutgoingEffect(EffectDefinition definition, int level = 1, EffectContext context = null) => MockSystem?.MakeOutgoingEffect(definition, level, context);
        public EffectContext MakeEffectContext() => MockSystem?.MakeEffectContext();
        public EffectApplicationResult ApplyEffectToSelf(Effect effect) => MockSystem?.ApplyEffectToSelf(effect) ?? EffectApplicationResult.Immune;
        public void Reset() => MockSystem?.Reset();
        public GameObject GetGameObjectFromNetworkId(ulong networkId) => MockSystem?.GetGameObjectFromNetworkId(networkId);
    }
}
