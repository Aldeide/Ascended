using System;
using UnityEngine;

namespace AbilitySystem.Runtime.Networking
{
    public interface INetworkRole
    {
        bool IsServer { get; }
        bool IsClient { get; }
        bool IsHost { get; }
        bool IsOwner { get; }
        bool IsLocalPlayer { get; }
        bool HasAuthority { get; }
        double Time { get; }
        ulong NetworkObjectId { get; }
        GameObject GetGameObjectFromNetworkId(ulong networkId);
    }
}
