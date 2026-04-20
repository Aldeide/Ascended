using System;
using UnityEngine;
using UnityEngine.VFX;

namespace AbilitySystem.Runtime.Cues
{
    [Serializable]
    [CreateAssetMenu(fileName = "PrefabCue", menuName = "AbilitySystem/Cues/PrefabCue")]
    public class CuePrefabDefinition : CueDefinition
    {
        public GameObject Prefab;
    }
}