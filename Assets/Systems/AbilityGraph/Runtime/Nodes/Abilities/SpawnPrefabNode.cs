using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Spawn/SpawnPrefab")]
    public class SpawnPrefabNode : LinearExecutableNode
    {
        public GameObject Prefab;
        
        [Input]
        public Transform Parent;
        [Input]
        public bool InstantiateInWorldSpace;

        [NonSerialized] private GameObject _cachedPrefab;
        [NonSerialized] private NetworkObject _cachedPrefabNetworkObject;
        [NonSerialized] private bool _prefabHasNetworkObjectCached;

        protected override void Process()
        {
            if (!Prefab || !Parent) return;

            // If the prefab has changed (or hasn't been cached yet), update our cache.
            if (_cachedPrefab != Prefab)
            {
                _cachedPrefab = Prefab;
                _cachedPrefabNetworkObject = Prefab.GetComponent<NetworkObject>();
                _prefabHasNetworkObjectCached = _cachedPrefabNetworkObject != null;
            }

            if (_prefabHasNetworkObjectCached)
            {
                // Instantiate directly using the NetworkObject to avoid GetComponent on the clone
                var instanceNetworkObject = Object.Instantiate(_cachedPrefabNetworkObject, Parent, InstantiateInWorldSpace);
                instanceNetworkObject.Spawn();
            }
            else
            {
                Object.Instantiate(Prefab, Parent, InstantiateInWorldSpace);
            }
        }
    }
}
