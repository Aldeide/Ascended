using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.AbilitySystem.Authoring
{
    public abstract class AbilityAsset : ScriptableObject
    {
        public string description;
        public Sprite icon;
    }
}