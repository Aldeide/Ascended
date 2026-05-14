using System.Collections.Generic;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Scripts;
using UnityEngine;

namespace AbilitySystem.Test.Utilities
{
    public interface INetworkSystemRegistry
    {
        IAbilitySystem GetSystemFromNetworkId(ulong networkId);
    }

    public class InteractionMockNetworkRole : INetworkRole
    {
        public bool IsServer { get; }
        public bool IsClient => !IsServer;
        public bool IsHost => IsServer && IsLocalPlayer;
        public bool IsOwner => IsLocalPlayer;
        public bool IsLocalPlayer { get; }
        public bool HasAuthority { get; }
        public double Time => UnityEngine.Time.timeAsDouble;
        public ulong NetworkObjectId { get; }
        
        private readonly INetworkSystemRegistry _registry;

        public InteractionMockNetworkRole(bool isServer, bool isLocalPlayer, ulong networkObjectId, INetworkSystemRegistry registry)
        {
            IsServer = isServer;
            IsLocalPlayer = isLocalPlayer;
            NetworkObjectId = networkObjectId;
            HasAuthority = isServer || isLocalPlayer;
            _registry = registry;
        }

        public GameObject GetGameObjectFromNetworkId(ulong networkId)
        {
            var system = GetSystemFromNetworkId(networkId);
            if (system == null) return null;

            var go = new GameObject($"MockPlayer_{networkId}");
            var asc = go.AddComponent<AbilitySystemComponent>();

            asc.AbilitySystem = system;

            return go;
        }
        
        public IAbilitySystem GetSystemFromNetworkId(ulong networkId)
        {
            return _registry?.GetSystemFromNetworkId(networkId);
        }
    }

    public class InteractionMockDataManager : IDataManager
    {
        public Dictionary<string, AbilityDefinition> Abilities = new();
        public Dictionary<string, EffectDefinition> Effects = new();

        public AbilityDefinition GetAbilityByName(string name) => Abilities.TryGetValue(name, out var def) ? def : null;
        public EffectDefinition GetEffectByName(string name) => Effects.TryGetValue(name, out var def) ? def : null;
        public CueDefinition GetCueByTag(GameplayTags.Runtime.Tag tag) => null;
        public CueDefinition GetCueByTag(string tag) => null;
    }
}
