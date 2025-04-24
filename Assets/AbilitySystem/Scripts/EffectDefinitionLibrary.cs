using System.Collections.Generic;
using AbilitySystem.Runtime.Effects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    public class EffectDefinitionLibrary : MonoBehaviour
    {
        [ShowInInspector]
        private Dictionary<string, EffectDefinition> _effects = new();

        private void Awake()
        {
            foreach (var effect in Resources.LoadAll<EffectDefinition>(""))
            {
                _effects.Add(effect.name, effect);
            }
        }

        public EffectDefinition GetEffectByName(string effectName)
        {
            return _effects.TryGetValue(effectName, out var effect) ? effect : null;
        }
    }
}