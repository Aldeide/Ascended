using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Effects;
using GameplayTags.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystem.Scripts
{
    public class DataLibrary : MonoBehaviour
    {
        public static DataLibrary Instance { get; private set; }
        [ShowInInspector] private Dictionary<string, AbilityDefinition> _abilities = new();
        [ShowInInspector] private Dictionary<string, EffectDefinition> _effects = new();
        [ShowInInspector] private Dictionary<Tag, CueDefinition> _cues = new();


        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var ability in Resources.LoadAll<AbilityDefinition>(""))
            {
                _abilities.Add(ability.UniqueName, ability);
            }

            foreach (var effect in Resources.LoadAll<EffectDefinition>(""))
            {
                _effects.Add(effect.name, effect);
            }

            foreach (var cue in Resources.LoadAll<CueDefinition>(""))
            {
                _cues.Add(cue.CueTag, cue);
            }
        }

        public CueDefinition GetCueByTag(Tag cueTag)
        {
            return _cues.GetValueOrDefault(cueTag);
        }

        public CueDefinition GetCueByTag(string cueTag)
        {
            return _cues.FirstOrDefault(c => c.Key.Name == cueTag).Value;
        }

        public AbilityDefinition GetAbilityByName(string abilityName)
        {
            return _abilities.GetValueOrDefault(abilityName);
        }
        
        public EffectDefinition GetEffectByName(string effectName)
        {
            return _effects.GetValueOrDefault(effectName);
        }
    }
}