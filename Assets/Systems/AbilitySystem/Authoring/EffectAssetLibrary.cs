using System.Collections;
using System.Collections.Generic;
using Systems.AbilitySystem.Effects;
using UnityEngine;

namespace Systems.AbilitySystem.Authoring
{
    public class EffectAssetLibrary : MonoBehaviour
    {
        [SerializeField]
        private Dictionary<string, EffectAsset> _effects = new();

        private void Awake()
        {
            foreach (var effect in Resources.LoadAll<EffectAsset>("Effects"))
            {
                _effects.Add(effect.name, effect);
            }
        }

        public EffectAsset GetEffectByName(string effectName)
        {
            return _effects.TryGetValue(effectName, out var effect) ? effect : null;
        }
        
    }
}