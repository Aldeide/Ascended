using System;
using AbilityGraph.Runtime.Nodes.Base;
using GraphProcessor;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AbilityGraph.Runtime.Nodes.Abilities
{
    [Serializable, NodeMenuItem("Spawn/SpawnPrefab")]
    public class SpawnPrefabNode : AbilityNode
    {
        public GameObject Prefab;
        
        [Input]
        public Transform Parent;
        [Input]
        public bool InstantiateInWorldSpace;
        
        protected override void Process()
        {
            if (!Prefab || !Parent) return;
            var instance = Object.Instantiate(Prefab, Parent, InstantiateInWorldSpace);
            // TODO: cache component fetching when graph is built.
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            if (instanceNetworkObject)
            {
                instanceNetworkObject.Spawn();
            }
        }
    }
}