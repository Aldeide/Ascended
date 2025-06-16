using System;
using UnityEngine;
using UnityEngine.VFX;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    [CreateAssetMenu(fileName = "SpawnPrefabCue", menuName = "AbilitySystem/Cues/SpawnPrefabCue")]
    public class SpawnPrefabCueDefinition : CueDefinition
    {
        public GameObject Prefab;
    }
}